using Microsoft.AspNetCore.Authorization;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public class ReadersController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = db.Readers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => EF.Functions.ILike(x.FullName, "%" + search.Trim() + "%"));
        ViewBag.Search = search;
        return View(await query.OrderBy(x => x.FullName).ToListAsync());
    }
    public async Task<IActionResult> Details(int id)
    {
        var entity = await db.Readers.AsNoTracking().Include(x => x.Requests).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize]
    public IActionResult Create() => View("Edit", new ReaderForm());
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(ReaderForm form)
    {
        if (!ModelState.IsValid) return View("Edit", form);
        db.Readers.Add(new Reader { FullName = form.FullName, Faculty = form.Faculty, Department = form.Department, Position = form.Position, Email = form.Email });
        await db.SaveChangesAsync();
        TempData["Success"] = "Запис додано";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await db.Readers.FindAsync(id);
        return entity is null ? NotFound() : View(new ReaderForm { FullName = entity.FullName, Faculty = entity.Faculty, Department = entity.Department, Position = entity.Position, Email = entity.Email });
    }
    [Authorize] [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, ReaderForm form)
    {
        var entity = await db.Readers.FindAsync(id);
        if (entity is null) return NotFound();
        if (!ModelState.IsValid) return View(form);
        entity.FullName = form.FullName;
        entity.Faculty = form.Faculty;
        entity.Department = form.Department;
        entity.Position = form.Position;
        entity.Email = form.Email;
        await db.SaveChangesAsync();
        TempData["Success"] = "Запис оновлено";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Readers.AsNoTracking().Include(x => x.Requests).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize] [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await db.Readers.FindAsync(id);
        if (entity is null) return NotFound();
        db.Readers.Remove(entity);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Запис видалено"; }
        catch (DbUpdateException) { TempData["Error"] = "Неможливо видалити: запис використовується в інших розділах"; }
        return RedirectToAction(nameof(Index));
    }
}
