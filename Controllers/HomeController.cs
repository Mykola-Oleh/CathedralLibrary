using CathedralLibrary.Data;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public sealed class HomeController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = new HomeDashboardViewModel
        {
            Publications = await db.Publications.CountAsync(ct),
            Authors = await db.Authors.CountAsync(ct),
            Readers = await db.Readers.CountAsync(ct),
            Copies = await db.Copies.CountAsync(ct),
            Requests = await db.Requests.CountAsync(ct),
            RecentPublications = await db.Publications.AsNoTracking()
                .Include(x => x.PublicationType)
                .Include(x => x.PublicationAuthors).ThenInclude(x => x.Author)
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                .Take(5).ToListAsync(ct)
        };
        return View(model);
    }
}
