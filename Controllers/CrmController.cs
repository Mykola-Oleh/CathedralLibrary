using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CathedralLibrary.Controllers;
[Authorize]
[ApiController]
[Route("api/crm")]
public sealed class CrmController(IHttpClientFactory factory, IConfiguration config) : ControllerBase
{
    public sealed record DemoContact(string Email, string FirstName, string LastName);
    [HttpPost("demo-contact")]
    public async Task<IActionResult> CreateDemoContact([FromBody] DemoContact input, CancellationToken ct)
    {
        var allowed = config.GetValue<bool>("Crm:Enabled");
        var token = config["Crm:HubSpotToken"];
        if (!allowed || string.IsNullOrWhiteSpace(token)) return StatusCode(503, new { error = "CRM інтеграція не налаштована" });
        if (string.IsNullOrWhiteSpace(input.Email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(input.Email)) return BadRequest();
        using var client = factory.CreateClient("hubspot");
        using var request = new HttpRequestMessage(HttpMethod.Post, "crm/v3/objects/contacts");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { properties = new { email = input.Email, firstname = input.FirstName, lastname = input.LastName } });
        try
        {
            using var result = await client.SendAsync(request, ct);
            var body = await result.Content.ReadAsStringAsync(ct);
            return StatusCode((int)result.StatusCode, body);
        }
        catch (HttpRequestException) { return StatusCode(502, new { error = "HubSpot недоступний" }); }
    }
}
