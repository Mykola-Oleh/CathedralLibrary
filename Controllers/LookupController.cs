using CathedralLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

[Route("lookup")]
public sealed class LookupController(LibraryDbContext db) : Controller
{
    [HttpGet("authors")]
    public async Task<IActionResult> Authors([FromQuery] string? term, CancellationToken ct)
    {
        term = term?.Trim();
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
            return Json(Array.Empty<object>());

        var items = await db.Authors.AsNoTracking()
            .Where(x => EF.Functions.ILike(x.FullName, "%" + term + "%"))
            .OrderBy(x => x.FullName).Take(20)
            .Select(x => new { id = x.Id, text = x.FullName })
            .ToListAsync(ct);
        return Json(items);
    }

    [HttpGet("publications")]
    public async Task<IActionResult> Publications([FromQuery] string? term, CancellationToken ct)
    {
        term = term?.Trim();
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
            return Json(Array.Empty<object>());

        var items = await db.Publications.AsNoTracking()
            .Where(x => EF.Functions.ILike(x.Title, "%" + term + "%"))
            .OrderBy(x => x.Title).Take(20)
            .Select(x => new { id = x.Id, text = x.Title })
            .ToListAsync(ct);
        return Json(items);
    }
}
