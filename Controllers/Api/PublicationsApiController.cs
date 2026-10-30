using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CathedralLibrary.Data;
using CathedralLibrary.Services;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers.Api;

public sealed record PublicationInput(
    [Required, StringLength(250)] string Title,
    [StringLength(3000)] string? Annotation,
    [Range(1450, 2100)] int Year,
    [StringLength(500), Url] string? CoverUrl,
    [Range(1, int.MaxValue)] int PublicationTypeId,
    List<int>? AuthorIds);

[Route("api/publications")]
public sealed class PublicationsApiController(LibraryDbContext db, LibraryCache cache) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        var query = db.Publications.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.ILike(x.Title, "%" + search.Trim() + "%"));
        if (skip < 0 || limit is < 1 or > 100) return BadRequest(new { error = "skip >= 0; limit від 1 до 100" });
        var version = cache.Version;
        var cacheKey = $"publications:list:{skip}:{limit}:{search?.Trim().ToLowerInvariant()}";
        var result = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var total = await query.CountAsync(ct);
            var items = await query.OrderBy(x => x.Id).Skip(skip).Take(limit)
                .Select(x => new { x.Id, x.Title, x.Annotation, x.Year, x.CoverUrl, x.PublicationTypeId,
                    AuthorIds = x.PublicationAuthors.Select(a => a.AuthorId).ToList(), x.CreatedAt, x.UpdatedAt })
                .ToListAsync(ct);
            var next = skip + items.Count < total
                ? Url.Action(nameof(GetAll), values: new { skip = skip + items.Count, limit, search })
                : null;
            return new { items, skip, limit, total, nextLink = next };
        });
        Response.Headers["X-Library-Cache-Version"] = version.ToString();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Publications.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new { x.Id, x.Title, x.Annotation, x.Year, x.CoverUrl, x.PublicationTypeId, AuthorIds = x.PublicationAuthors.Select(a => a.AuthorId).ToList(), x.CreatedAt, x.UpdatedAt })
            .FirstOrDefaultAsync(ct);
        return item is null ? NotFound() : Ok(item);
    }

    private async Task<bool> ReferencesValid(PublicationInput input, CancellationToken ct)
    {
        if (!await db.PublicationTypes.AnyAsync(x => x.Id == input.PublicationTypeId, ct)) return false;
        var ids = (input.AuthorIds ?? new()).Distinct().ToArray();
        return ids.Length == await db.Authors.CountAsync(x => ids.Contains(x.Id), ct);
    }

    [Authorize] [HttpPost]
    public async Task<IActionResult> Create([FromBody] PublicationInput input, CancellationToken ct)
    {
        if (!await ReferencesValid(input, ct)) return BadRequest(new { error = "Невідомий тип видання або автор." });
        var item = new Publication { Title = input.Title, Annotation = input.Annotation, Year = input.Year, CoverUrl = input.CoverUrl, PublicationTypeId = input.PublicationTypeId };
        foreach (var authorId in (input.AuthorIds ?? new()).Distinct())
            item.PublicationAuthors.Add(new PublicationAuthor { AuthorId = authorId });
        db.Publications.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, new { item.Id });
    }

    [Authorize] [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PublicationInput input, CancellationToken ct)
    {
        var item = await db.Publications.Include(x => x.PublicationAuthors).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return NotFound();
        if (!await ReferencesValid(input, ct)) return BadRequest(new { error = "Невідомий тип видання або автор." });
        item.Title = input.Title; item.Annotation = input.Annotation; item.Year = input.Year;
        item.CoverUrl = input.CoverUrl; item.PublicationTypeId = input.PublicationTypeId;
        db.PublicationAuthors.RemoveRange(item.PublicationAuthors);
        foreach (var authorId in (input.AuthorIds ?? new()).Distinct())
            db.PublicationAuthors.Add(new PublicationAuthor { PublicationId = id, AuthorId = authorId });
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize] [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Publications.FindAsync(new object[] { id }, ct);
        if (item is null) return NotFound();
        if (await db.Copies.AnyAsync(x => x.PublicationId == id, ct)) return ConflictDelete();
        db.Publications.Remove(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException) { return ConflictDelete(); }
        return NoContent();
    }
}
