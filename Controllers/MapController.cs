using CathedralLibrary.Models;
using Microsoft.AspNetCore.Mvc;
namespace CathedralLibrary.Controllers;
public sealed class MapController(IConfiguration config) : Controller
{
    public IActionResult Index() => View();
    [HttpGet("api/library-locations")]
    public IActionResult Locations([FromQuery] string? category = null)
    {
        var locations = config.GetSection("LibraryLocations").Get<List<LibraryLocation>>() ?? new();
        return Ok(locations.Where(x => (string.IsNullOrWhiteSpace(category) || x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
             && x.Latitude is >= -90 and <= 90 && x.Longitude is >= -180 and <= 180));
    }
}
