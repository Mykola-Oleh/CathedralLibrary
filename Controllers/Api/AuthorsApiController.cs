using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

public sealed record AuthorInput([Required, StringLength(160)] string FullName, [StringLength(1000)] string? Biography);

[Route("api/authors")]
public sealed class AuthorsApiController(LibraryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, CancellationToken ct = default)
        => await Page(db.Authors.AsNoTracking().OrderBy(x => x.FullName), skip, limit, ct);

    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Authors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [Authorize] [HttpPost] public async Task<IActionResult> Create([FromBody] AuthorInput input, CancellationToken ct)
    {
        var item = new Author { FullName = input.FullName, Biography = input.Biography };
        db.Authors.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [Authorize] [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, [FromBody] AuthorInput input, CancellationToken ct)
    {
        var item = await db.Authors.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        item.FullName = input.FullName; item.Biography = input.Biography;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize] [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Authors.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (await db.PublicationAuthors.AnyAsync(x => x.AuthorId == id, ct)) return ConflictDelete();
        db.Authors.Remove(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return ConflictDelete(); }
        return NoContent();
    }
}
