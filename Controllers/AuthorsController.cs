using Microsoft.AspNetCore.Authorization;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public class AuthorsController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = db.Authors.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => EF.Functions.ILike(x.FullName, "%" + search.Trim() + "%"));
        ViewBag.Search = search;
        return View(await query.OrderBy(x => x.FullName).ToListAsync());
    }
    public async Task<IActionResult> Details(int id)
    {
        var entity = await db.Authors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize]
    public IActionResult Create() => View("Edit", new AuthorForm());
    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(AuthorForm form)
    {
        if (!ModelState.IsValid) return View("Edit", form);
        db.Authors.Add(new Author { FullName = form.FullName, Biography = form.Biography });
        await db.SaveChangesAsync();
        TempData["Success"] = "Запис додано";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await db.Authors.FindAsync(id);
        return entity is null ? NotFound() : View(new AuthorForm { FullName = entity.FullName, Biography = entity.Biography });
    }
    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, AuthorForm form)
    {
        var entity = await db.Authors.FindAsync(id);
        if (entity is null) return NotFound();
        if (!ModelState.IsValid) return View(form);
        entity.FullName = form.FullName;
        entity.Biography = form.Biography;
        await db.SaveChangesAsync();
        TempData["Success"] = "Запис оновлено";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Authors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize]
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await db.Authors.FindAsync(id);
        if (entity is null) return NotFound();
        db.Authors.Remove(entity);
        try { await db.SaveChangesAsync(); TempData["Success"] = "Запис видалено"; }
        catch (DbUpdateException) { TempData["Error"] = "Неможливо видалити: запис використовується в інших розділах"; }
        return RedirectToAction(nameof(Index));
    }
}
