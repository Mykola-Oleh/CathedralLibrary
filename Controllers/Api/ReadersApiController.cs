using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

public sealed record ReaderInput([Required, StringLength(160)] string FullName, [StringLength(120)] string Faculty, [StringLength(120)] string Department, [StringLength(100)] string Position, [EmailAddress, StringLength(254)] string? Email);

[Route("api/readers")]
public sealed class ReadersApiController(LibraryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, CancellationToken ct = default)
        => await Page(db.Readers.AsNoTracking().OrderBy(x => x.FullName), skip, limit, ct);

    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Readers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [Authorize] [HttpPost] public async Task<IActionResult> Create([FromBody] ReaderInput input, CancellationToken ct)
    {
        var item = new Reader { FullName = input.FullName, Faculty = input.Faculty, Department = input.Department, Position = input.Position, Email = input.Email };
        db.Readers.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [Authorize] [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, [FromBody] ReaderInput input, CancellationToken ct)
    {
        var item = await db.Readers.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        item.FullName = input.FullName; item.Faculty = input.Faculty; item.Department = input.Department; item.Position = input.Position; item.Email = input.Email;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize] [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Readers.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (await db.Requests.AnyAsync(x => x.ReaderId == id, ct)) return ConflictDelete();
        db.Readers.Remove(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return ConflictDelete(); }
        return NoContent();
    }
}
