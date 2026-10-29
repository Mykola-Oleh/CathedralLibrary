using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CathedralLibrary.Controllers;

[Route("account")]
public class AccountController(IConfiguration config) : Controller
{
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return LocalRedirect(SafeUrl(returnUrl));
        if (string.IsNullOrWhiteSpace(config["Authentication:Google:ClientId"]) ||
            string.IsNullOrWhiteSpace(config["Authentication:Google:ClientSecret"]))
            return Problem("Google OAuth не налаштовано. Див. README-Day4.md", statusCode: 503);
        return Challenge(new AuthenticationProperties { RedirectUri = SafeUrl(returnUrl) }, GoogleDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("denied")]
    public IActionResult Denied() => StatusCode(403, "Доступ заборонено");

    private string SafeUrl(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
}
