using CathedralLibrary.Data;
using CathedralLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CathedralLibrary.Controllers.Api;
[ApiController]
[Route("api/search")]
public sealed class SearchApiController(LibraryDbContext db, LucenePublicationSearch search) : ControllerBase
{
    [HttpGet("publications")]
    public async Task<IActionResult> Publications([FromQuery] string? q, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2 || q.Length > 100 || limit is < 1 or > 50)
            return BadRequest(new { error = "q: 2–100 символів; limit: 1–50" });
        var ids = await search.FindIdsAsync(db, q, limit, ct);
        var records = await db.Publications.AsNoTracking().Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.Title, x.Year, x.Annotation }).ToListAsync(ct);
        var byId = records.ToDictionary(x => x.Id);
        return Ok(ids.Where(byId.ContainsKey).Select(id => byId[id]));
    }
}
