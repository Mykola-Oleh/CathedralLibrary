using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace CathedralLibrary.Services;
public static class LibraryWebSocketHandler
{
    public static async Task Handle(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest) { context.Response.StatusCode = 400; return; }
        var feed = context.RequestServices.GetRequiredService<LibraryEventFeed>();
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        long last = 0;
        while (socket.State == WebSocketState.Open && !context.RequestAborted.IsCancellationRequested)
        {
            var events = feed.Since(last);
            if (events.Length > 0)
            {
                last = events[^1].Id;
                var payload = JsonSerializer.Serialize(new { version = feed.Current, events });
                var bytes = Encoding.UTF8.GetBytes(payload);
                await socket.SendAsync(bytes.AsMemory(), WebSocketMessageType.Text, true, context.RequestAborted);
            }
            else
            {
                await feed.WaitAsync(last, TimeSpan.FromSeconds(25), context.RequestAborted);
                if (socket.State != WebSocketState.Open) break;
                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { version = feed.Current, events = Array.Empty<object>() }));
                await socket.SendAsync(bytes.AsMemory(), WebSocketMessageType.Text, true, context.RequestAborted);
            }
        }
    }
}
