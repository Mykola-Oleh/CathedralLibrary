using System.Security.Cryptography;
using System.Text;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CathedralLibrary.Controllers.Api;
[ApiController]
[Route("api/mcp-bridge")]
public sealed class McpBridgeController(LibraryDbContext db, IConfiguration config, IWebHostEnvironment env) : ControllerBase
{
    public sealed record CreatePublicationInput(string Title, int Year, int PublicationTypeId, string? Annotation);
    [HttpPost("publications")]
    public async Task<IActionResult> Create([FromBody] CreatePublicationInput input, CancellationToken ct)
    {
        if (!config.GetValue<bool>("McpBridge:Enabled")) return NotFound();
        if (!Request.IsHttps && !env.IsDevelopment()) return StatusCode(403, new { error = "HTTPS required" });
        var expected = config["McpBridge:ApiKey"];
        var provided = Request.Headers["X-Mcp-Key"].ToString();
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(provided)) return Unauthorized();
        var left = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        var right = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
        if (!CryptographicOperations.FixedTimeEquals(left, right)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(input.Title) || input.Title.Length > 250 || input.Year is < 1450 or > 2100 ||
            input.Annotation?.Length > 3000 || !await db.PublicationTypes.AnyAsync(x => x.Id == input.PublicationTypeId, ct))
            return BadRequest(new { error = "Некоректні дані або невідомий тип видання" });
        var publication = new Publication { Title = input.Title, Year = input.Year, PublicationTypeId = input.PublicationTypeId, Annotation = input.Annotation };
        db.Publications.Add(publication);
        await db.SaveChangesAsync(ct);
        return Created($"/api/publications/{publication.Id}", new { publication.Id });
    }
}
