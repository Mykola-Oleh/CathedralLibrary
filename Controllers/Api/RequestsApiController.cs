using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

public sealed record RequestInput([Range(1, int.MaxValue)] int ReaderId, [Range(1, int.MaxValue)] int CopyId,
    [Range(1, int.MaxValue)] int RequestStatusId, DateTime? ReturnedAt);

[Route("api/requests")]
public sealed class RequestsApiController(LibraryDbContext db) : ApiControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, CancellationToken ct = default)
        => await Page(db.Requests.AsNoTracking().OrderBy(x => x.Id), skip, limit, ct);
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Requests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }
    private async Task<bool> Valid(RequestInput input, CancellationToken ct)
        => await db.Readers.AnyAsync(x => x.Id == input.ReaderId, ct)
           && await db.Copies.AnyAsync(x => x.Id == input.CopyId, ct)
           && await db.RequestStatuses.AnyAsync(x => x.Id == input.RequestStatusId, ct);
    [Authorize] [HttpPost] public async Task<IActionResult> Create([FromBody] RequestInput input, CancellationToken ct)
    {
        if (!await Valid(input, ct)) return BadRequest(new { error = "Невірний читач, примірник або статус." });
        var item = new LibraryRequest { ReaderId = input.ReaderId, CopyId = input.CopyId, RequestStatusId = input.RequestStatusId, ReturnedAt = input.ReturnedAt };
        db.Requests.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, new { item.Id });
    }
    [Authorize] [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, [FromBody] RequestInput input, CancellationToken ct)
    {
        var item = await db.Requests.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (!await Valid(input, ct)) return BadRequest(new { error = "Невірний читач, примірник або статус." });
        item.ReaderId = input.ReaderId; item.CopyId = input.CopyId; item.RequestStatusId = input.RequestStatusId; item.ReturnedAt = input.ReturnedAt;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
    [Authorize] [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Requests.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        db.Requests.Remove(item);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
