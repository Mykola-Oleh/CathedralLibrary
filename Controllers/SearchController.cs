using CathedralLibrary.Data;
using CathedralLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CathedralLibrary.Controllers;
public sealed class SearchController(LibraryDbContext db, LucenePublicationSearch lucene) : Controller
{
    [HttpGet("/search")]
    public async Task<IActionResult> Index(string? q, CancellationToken ct)
    {
        ViewBag.Query = q;
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2 || q.Length > 100)
            return View(new List<CathedralLibrary.Models.Publication>());
        var ids = await lucene.FindIdsAsync(db, q, 30, ct);
        var rows = await db.Publications.AsNoTracking().Where(p => ids.Contains(p.Id))
            .Include(p => p.PublicationAuthors).ThenInclude(a => a.Author).ToListAsync(ct);
        var lookup = rows.ToDictionary(x => x.Id);
        return View(ids.Where(lookup.ContainsKey).Select(id => lookup[id]).ToList());
    }
}
