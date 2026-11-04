using CathedralLibrary.Data;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.EntityFrameworkCore;
namespace CathedralLibrary.Services;
public sealed class LucenePublicationSearch(LibraryCache cache)
{
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private long _indexedVersion = -1;
    private RAMDirectory? _directory;
    private readonly LuceneVersion _version = LuceneVersion.LUCENE_48;
    public async Task<IReadOnlyList<int>> FindIdsAsync(LibraryDbContext db, string term, int limit, CancellationToken ct)
    {
        await _mutex.WaitAsync(ct);
        try
        {
            if (_indexedVersion != cache.Version || _directory is null)
            {
                var records = await db.Publications.AsNoTracking()
                    .Select(x => new { x.Id, x.Title, x.Annotation, Authors = x.PublicationAuthors.Select(a => a.Author.FullName).ToList() }).ToListAsync(ct);
                var newDirectory = new RAMDirectory();
                using (var analyzer = new StandardAnalyzer(_version))
                using (var writer = new IndexWriter(newDirectory, new IndexWriterConfig(_version, analyzer)))
                {
                    foreach (var x in records)
                    {
                        var doc = new Document
                        {
                            new StringField("id", x.Id.ToString(), Field.Store.YES),
                            new TextField("title", x.Title, Field.Store.NO),
                            new TextField("annotation", x.Annotation ?? "", Field.Store.NO),
                            new TextField("authors", string.Join(" ", x.Authors), Field.Store.NO)
                        };
                        writer.AddDocument(doc);
                    }
                }
                _directory?.Dispose(); _directory = newDirectory; _indexedVersion = cache.Version;
            }
            using var reader = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(reader);
            using var analyzer2 = new StandardAnalyzer(_version);
            var parser = new MultiFieldQueryParser(_version, new[] { "title", "annotation", "authors" }, analyzer2);
            var query = parser.Parse(QueryParser.Escape(term.Trim()));
            return searcher.Search(query, limit).ScoreDocs.Select(hit => int.Parse(searcher.Doc(hit.Doc).Get("id"))).ToArray();
        }
        finally { _mutex.Release(); }
    }
}
