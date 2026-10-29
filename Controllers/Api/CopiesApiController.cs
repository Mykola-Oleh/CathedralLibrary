using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

public sealed record CopyInput([Range(1, int.MaxValue)] int PublicationId, [Range(1, int.MaxValue)] int CopyNumber, [Range(1, int.MaxValue)] int CopyStatusId);

[Route("api/copies")]
public sealed class CopiesApiController(LibraryDbContext db) : ApiControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, CancellationToken ct = default)
        => await Page(db.Copies.AsNoTracking().OrderBy(x => x.Id), skip, limit, ct);
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Copies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }
    private async Task<bool> Valid(CopyInput input, int? excludedId, CancellationToken ct)
        => await db.Publications.AnyAsync(x => x.Id == input.PublicationId, ct)
           && await db.CopyStatuses.AnyAsync(x => x.Id == input.CopyStatusId, ct)
           && !await db.Copies.AnyAsync(x => x.PublicationId == input.PublicationId && x.CopyNumber == input.CopyNumber && x.Id != excludedId, ct);
    [Authorize] [HttpPost] public async Task<IActionResult> Create([FromBody] CopyInput input, CancellationToken ct)
    {
        if (!await Valid(input, null, ct)) return BadRequest(new { error = "Невірні посилання або номер примірника вже існує." });
        var item = new Copy { PublicationId = input.PublicationId, CopyNumber = input.CopyNumber, CopyStatusId = input.CopyStatusId };
        db.Copies.Add(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return Conflict(new { error = "Неможливо створити примірник." }); }
        return CreatedAtAction(nameof(Get), new { id = item.Id }, new { item.Id });
    }
    [Authorize] [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, [FromBody] CopyInput input, CancellationToken ct)
    {
        var item = await db.Copies.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (!await Valid(input, id, ct)) return BadRequest(new { error = "Невірні посилання або номер примірника вже існує." });
        item.PublicationId = input.PublicationId; item.CopyNumber = input.CopyNumber; item.CopyStatusId = input.CopyStatusId;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return Conflict(new { error = "Неможливо оновити примірник." }); }
        return NoContent();
    }
    [Authorize] [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Copies.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (await db.Requests.AnyAsync(x => x.CopyId == id, ct)) return ConflictDelete();
        db.Copies.Remove(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return ConflictDelete(); }
        return NoContent();
    }
}
