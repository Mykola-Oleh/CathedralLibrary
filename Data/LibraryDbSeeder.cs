using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Data;

public static class LibraryDbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        if (await db.Publications.AnyAsync()) return;

        var author = new Models.Author { FullName = "Шевченко Тарас Григорович", Biography = "Український письменник і поет." };
        var publication = new Models.Publication
        {
            Title = "Кобзар",
            Annotation = "Збірка поетичних творів Тараса Шевченка.",
            Year = 2020,
            PublicationTypeId = 1,
            CoverUrl = "https://example.com/kobzar-cover.jpg"
        };
        db.Authors.Add(author);
        db.Publications.Add(publication);
        await db.SaveChangesAsync();

        db.PublicationAuthors.Add(new Models.PublicationAuthor { AuthorId = author.Id, PublicationId = publication.Id });
        db.Copies.Add(new Models.Copy { PublicationId = publication.Id, CopyNumber = 1, CopyStatusId = 1 });
        db.FundChanges.Add(new Models.FundChange
        {
            PublicationId = publication.Id,
            Description = "Початкове внесення видання до демонстраційного фонду.",
            CopiesDelta = 1,
            ChangedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
