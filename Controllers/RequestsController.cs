using Microsoft.AspNetCore.Authorization;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public class RequestsController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Requests.AsNoTracking().Include(x => x.Reader).Include(x => x.Copy).ThenInclude(x => x.Publication).Include(x => x.RequestStatus).OrderByDescending(x => x.RequestedAt).ToListAsync());
    public async Task<IActionResult> Details(int id)
    {
        var entity = await db.Requests.AsNoTracking().Include(x => x.Reader).Include(x => x.Copy).ThenInclude(x => x.Publication).Include(x => x.RequestStatus).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    private async Task LoadLists(RequestForm form)
    {
        ViewBag.Readers = new SelectList(await db.Readers.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(), "Id", "FullName", form.ReaderId);
        ViewBag.Copies = new SelectList(await db.Copies.AsNoTracking().Include(x => x.Publication).OrderBy(x => x.Publication.Title).ThenBy(x => x.CopyNumber).Select(x => new { x.Id, Name = x.Publication.Title + " — №" + x.CopyNumber }).ToListAsync(), "Id", "Name", form.CopyId);
        ViewBag.Statuses = new SelectList(await db.RequestStatuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(), "Id", "Name", form.RequestStatusId);
    }
    private async Task Validate(RequestForm form)
    {
        if (!await db.Readers.AnyAsync(x => x.Id == form.ReaderId)) ModelState.AddModelError(nameof(form.ReaderId), "Читач не існує");
        if (!await db.Copies.AnyAsync(x => x.Id == form.CopyId)) ModelState.AddModelError(nameof(form.CopyId), "Примірник не існує");
        if (!await db.RequestStatuses.AnyAsync(x => x.Id == form.RequestStatusId)) ModelState.AddModelError(nameof(form.RequestStatusId), "Статус не існує");
        if (form.ReturnedAt.HasValue && form.ReturnedAt.Value.Date > DateTime.UtcNow.Date) ModelState.AddModelError(nameof(form.ReturnedAt), "Дата повернення не може бути в майбутньому");
    }
    [Authorize]
    public async Task<IActionResult> Create() { var form = new RequestForm { RequestStatusId = 1 }; await LoadLists(form); return View("Edit", form); }
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(RequestForm form)
    {
        await Validate(form);
        if (!ModelState.IsValid) { await LoadLists(form); return View("Edit", form); }
        db.Requests.Add(new LibraryRequest { ReaderId = form.ReaderId, CopyId = form.CopyId, RequestStatusId = form.RequestStatusId, ReturnedAt = form.ReturnedAt.HasValue ? DateTime.SpecifyKind(form.ReturnedAt.Value.Date, DateTimeKind.Utc) : null });
        await db.SaveChangesAsync(); TempData["Success"] = "Запит створено"; return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await db.Requests.FindAsync(id);
        if (entity is null) return NotFound();
        var form = new RequestForm { ReaderId = entity.ReaderId, CopyId = entity.CopyId, RequestStatusId = entity.RequestStatusId, ReturnedAt = entity.ReturnedAt?.Date };
        await LoadLists(form); return View(form);
    }
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, RequestForm form)
    {
        var entity = await db.Requests.FindAsync(id);
        if (entity is null) return NotFound();
        await Validate(form);
        if (form.ReturnedAt.HasValue && form.ReturnedAt.Value.Date < entity.RequestedAt.Date) ModelState.AddModelError(nameof(form.ReturnedAt), "Дата повернення раніше за дату запиту");
        if (!ModelState.IsValid) { await LoadLists(form); return View(form); }
        entity.ReaderId = form.ReaderId; entity.CopyId = form.CopyId; entity.RequestStatusId = form.RequestStatusId;
        entity.ReturnedAt = form.ReturnedAt.HasValue ? DateTime.SpecifyKind(form.ReturnedAt.Value.Date, DateTimeKind.Utc) : null;
        await db.SaveChangesAsync(); TempData["Success"] = "Запит оновлено"; return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Requests.AsNoTracking().Include(x => x.Reader).Include(x => x.Copy).ThenInclude(x => x.Publication).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize] [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await db.Requests.FindAsync(id);
        if (entity is null) return NotFound();
        db.Requests.Remove(entity); await db.SaveChangesAsync(); TempData["Success"] = "Запит видалено"; return RedirectToAction(nameof(Index));
    }
}
