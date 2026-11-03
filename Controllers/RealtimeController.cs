using CathedralLibrary.Services;
using Microsoft.AspNetCore.Mvc;
namespace CathedralLibrary.Controllers;
public sealed class RealtimeController(LibraryEventFeed feed) : Controller
{
    [HttpGet] public IActionResult Index() => View();
    [HttpGet("api/realtime/poll")]
    public IActionResult Poll([FromQuery] long after = 0) => Ok(new { version = feed.Current, events = feed.Since(after) });
    [HttpGet("api/realtime/long-poll")]
    public async Task<IActionResult> LongPoll([FromQuery] long after = 0, CancellationToken ct = default)
    {
        await feed.WaitAsync(after, TimeSpan.FromSeconds(25), ct);
        return Ok(new { version = feed.Current, events = feed.Since(after) });
    }
}
