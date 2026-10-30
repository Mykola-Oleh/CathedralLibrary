using CathedralLibrary.Data;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public sealed class StatisticsController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var types = await db.PublicationTypes.AsNoTracking()
            .Select(t => new { t.Name, Count = t.Publications.Count })
            .OrderBy(x => x.Name).ToListAsync(ct);
        var statuses = await db.CopyStatuses.AsNoTracking()
            .Select(s => new { s.Name, Count = s.Copies.Count })
            .OrderBy(x => x.Name).ToListAsync(ct);
        var vm = new StatisticsViewModel
        {
            Publications = await db.Publications.CountAsync(ct),
            Authors = await db.Authors.CountAsync(ct),
            Readers = await db.Readers.CountAsync(ct),
            Copies = await db.Copies.CountAsync(ct),
            TypeNames = types.Select(x => x.Name).ToList(),
            TypeCounts = types.Select(x => x.Count).ToList(),
            StatusNames = statuses.Select(x => x.Name).ToList(),
            StatusCounts = statuses.Select(x => x.Count).ToList()
        };
        return View(vm);
    }
}
