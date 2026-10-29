using CathedralLibrary.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected async Task<IActionResult> Page<T>(IQueryable<T> query, int skip, int limit, CancellationToken ct)
    {
        if (skip < 0 || limit is < 1 or > 100)
            return BadRequest(new { error = "skip має бути >= 0, limit — від 1 до 100." });
        var total = await query.CountAsync(ct);
        var items = await query.Skip(skip).Take(limit).ToListAsync(ct);
        var next = skip + items.Count < total
            ? Url.Action(null, values: new { skip = skip + items.Count, limit })
            : null;
        return Ok(new PagedResponse<T>(items, skip, limit, total, next));
    }

    protected IActionResult ConflictDelete() => Conflict(new { error = "Запис використовується іншими сутностями." });
}
