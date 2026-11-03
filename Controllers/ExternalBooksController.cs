using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
namespace CathedralLibrary.Controllers;
[Route("api/external-books")]
public sealed class ExternalBooksController(IHttpClientFactory factory) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 3 || title.Length > 120)
            return BadRequest(new { error = "Назва: від 3 до 120 символів" });
        try
        {
            using var client = factory.CreateClient("openlibrary");
            using var response = await client.GetAsync("search.json?title=" + Uri.EscapeDataString(title.Trim()) + "&limit=5&fields=key,title,author_name,first_publish_year", ct);
            if (!response.IsSuccessStatusCode) return StatusCode(502, new { error = "Open Library недоступний" });
            using var data = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonDocument>(cancellationToken: ct);
            return data is null ? StatusCode(502) : Content(data.RootElement.GetRawText(), "application/json");
        }
        catch (HttpRequestException) { return StatusCode(502, new { error = "Помилка зовнішнього API" }); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) { return StatusCode(504, new { error = "Тайм-аут зовнішнього API" }); }
    }
}
