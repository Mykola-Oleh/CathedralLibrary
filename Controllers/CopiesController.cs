using Microsoft.AspNetCore.Authorization;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public class CopiesController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Copies.AsNoTracking().Include(x => x.Publication).Include(x => x.CopyStatus).OrderBy(x => x.Publication.Title).ThenBy(x => x.CopyNumber).ToListAsync());
    public async Task<IActionResult> Details(int id)
    {
        var entity = await db.Copies.AsNoTracking().Include(x => x.Publication).Include(x => x.CopyStatus).Include(x => x.Requests).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    private async Task LoadLists(CopyForm form)
    {
        ViewBag.Publications = new SelectList(await db.Publications.AsNoTracking().OrderBy(x => x.Title).ToListAsync(), "Id", "Title", form.PublicationId);
        ViewBag.Statuses = new SelectList(await db.CopyStatuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(), "Id", "Name", form.CopyStatusId);
    }
    private async Task Validate(CopyForm form, int? id = null)
    {
        if (!await db.Publications.AnyAsync(x => x.Id == form.PublicationId)) ModelState.AddModelError(nameof(form.PublicationId), "Видання не існує");
        if (!await db.CopyStatuses.AnyAsync(x => x.Id == form.CopyStatusId)) ModelState.AddModelError(nameof(form.CopyStatusId), "Статус не існує");
        if (await db.Copies.AnyAsync(x => x.PublicationId == form.PublicationId && x.CopyNumber == form.CopyNumber && (!id.HasValue || x.Id != id.Value))) ModelState.AddModelError(nameof(form.CopyNumber), "Цей номер примірника вже існує для видання");
    }
    [Authorize]
    public async Task<IActionResult> Create() { var form = new CopyForm { CopyStatusId = 1 }; await LoadLists(form); return View("Edit", form); }
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CopyForm form)
    {
        await Validate(form);
        if (!ModelState.IsValid) { await LoadLists(form); return View("Edit", form); }
        var entity = new Copy { PublicationId = form.PublicationId, CopyNumber = form.CopyNumber, CopyStatusId = form.CopyStatusId };
        db.Copies.Add(entity);
        db.FundChanges.Add(new FundChange { PublicationId = form.PublicationId, Description = $"Додано примірник №{form.CopyNumber}", CopiesDelta = 1 });
        try { await db.SaveChangesAsync(); TempData["Success"] = "Примірник додано"; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Не вдалося зберегти примірник. Перевірте його номер."); await LoadLists(form); return View("Edit", form); }
    }
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await db.Copies.FindAsync(id);
        if (entity is null) return NotFound();
        var form = new CopyForm { PublicationId = entity.PublicationId, CopyNumber = entity.CopyNumber, CopyStatusId = entity.CopyStatusId };
        await LoadLists(form); return View(form);
    }
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, CopyForm form)
    {
        var entity = await db.Copies.FindAsync(id);
        if (entity is null) return NotFound();
        await Validate(form, id);
        if (!ModelState.IsValid) { await LoadLists(form); return View(form); }
        if (await db.Requests.AnyAsync(x => x.CopyId == id) && (entity.PublicationId != form.PublicationId || entity.CopyNumber != form.CopyNumber))
        { ModelState.AddModelError("", "Не можна змінити видання або номер примірника, для якого є запити"); await LoadLists(form); return View(form); }
        if (entity.PublicationId != form.PublicationId)
        {
            db.FundChanges.Add(new FundChange { PublicationId = entity.PublicationId, Description = $"Перенесено примірник №{entity.CopyNumber} до іншого видання", CopiesDelta = -1 });
            db.FundChanges.Add(new FundChange { PublicationId = form.PublicationId, Description = $"Отримано примірник №{form.CopyNumber} з іншого видання", CopiesDelta = 1 });
        }
        entity.PublicationId = form.PublicationId; entity.CopyNumber = form.CopyNumber; entity.CopyStatusId = form.CopyStatusId;
        try { await db.SaveChangesAsync(); TempData["Success"] = "Примірник оновлено"; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Не вдалося зберегти зміни"); await LoadLists(form); return View(form); }
    }
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Copies.AsNoTracking().Include(x => x.Publication).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize] [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await db.Copies.FindAsync(id);
        if (entity is null) return NotFound();
        if (await db.Requests.AnyAsync(x => x.CopyId == id)) TempData["Error"] = "Примірник має історію запитів, видалення заборонено";
        else
        {
            db.FundChanges.Add(new FundChange { PublicationId = entity.PublicationId, Description = $"Вилучено примірник №{entity.CopyNumber}", CopiesDelta = -1 });
            db.Copies.Remove(entity);
            await db.SaveChangesAsync(); TempData["Success"] = "Примірник видалено";
        }
        return RedirectToAction(nameof(Index));
    }
}
