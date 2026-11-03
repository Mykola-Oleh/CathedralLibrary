using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CathedralLibrary.Controllers;
[Authorize]
public sealed class NotesController : Controller
{
    [HttpGet] public IActionResult Index() => View();
}
