using Microsoft.AspNetCore.Authorization;
using CathedralLibrary.Data;
using CathedralLibrary.Models;
using CathedralLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Controllers;

public class PublicationsController(LibraryDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = db.Publications.AsNoTracking().Include(x => x.PublicationType).Include(x => x.PublicationAuthors).ThenInclude(x => x.Author).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = "%" + search.Trim() + "%";
            query = query.Where(x => EF.Functions.ILike(x.Title, term) || x.PublicationAuthors.Any(a => EF.Functions.ILike(a.Author.FullName, term)));
        }
        ViewBag.Search = search;
        return View(await query.OrderBy(x => x.Title).ToListAsync());
    }
    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Publications.AsNoTracking().Include(x => x.PublicationType).Include(x => x.PublicationAuthors).ThenInclude(x => x.Author).Include(x => x.Copies).ThenInclude(x => x.CopyStatus).Include(x => x.FundChanges).FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }
    private async Task LoadLists(PublicationForm form)
    {
        ViewBag.Types = new SelectList(await db.PublicationTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(), "Id", "Name", form.PublicationTypeId);
        ViewBag.SelectedAuthors = await db.Authors.AsNoTracking()
            .Where(x => form.AuthorIds.Contains(x.Id)).OrderBy(x => x.FullName).ToListAsync();
    }
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var form = new PublicationForm(); await LoadLists(form); return View("Edit", form);
    }
    private async Task<bool> ValidateForm(PublicationForm form)
    {
        if (!await db.PublicationTypes.AnyAsync(x => x.Id == form.PublicationTypeId)) ModelState.AddModelError(nameof(form.PublicationTypeId), "Тип не існує");
        var ids = form.AuthorIds.Distinct().ToArray();
        if (ids.Length != await db.Authors.CountAsync(x => ids.Contains(x.Id))) ModelState.AddModelError(nameof(form.AuthorIds), "Один із авторів не існує");
        return ModelState.IsValid;
    }
    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(PublicationForm form)
    {
        if (!await ValidateForm(form)) { await LoadLists(form); return View("Edit", form); }
        var entity = new Publication { Title = form.Title, Annotation = form.Annotation, Year = form.Year, CoverUrl = form.CoverUrl, PublicationTypeId = form.PublicationTypeId };
        foreach (var id in form.AuthorIds.Distinct()) entity.PublicationAuthors.Add(new PublicationAuthor { AuthorId = id });
        db.Publications.Add(entity);
        await db.SaveChangesAsync();
        TempData["Success"] = "Видання додано";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await db.Publications.Include(x => x.PublicationAuthors).FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        var form = new PublicationForm { Title = entity.Title, Annotation = entity.Annotation, Year = entity.Year, CoverUrl = entity.CoverUrl, PublicationTypeId = entity.PublicationTypeId, AuthorIds = entity.PublicationAuthors.Select(x => x.AuthorId).ToList() };
        await LoadLists(form); return View(form);
    }
    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, PublicationForm form)
    {
        var entity = await db.Publications.Include(x => x.PublicationAuthors).FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        if (!await ValidateForm(form)) { await LoadLists(form); return View(form); }
        entity.Title = form.Title; entity.Annotation = form.Annotation; entity.Year = form.Year;
        entity.CoverUrl = form.CoverUrl; entity.PublicationTypeId = form.PublicationTypeId;
        db.PublicationAuthors.RemoveRange(entity.PublicationAuthors);
        foreach (var authorId in form.AuthorIds.Distinct()) db.PublicationAuthors.Add(new PublicationAuthor { PublicationId = id, AuthorId = authorId });
        await db.SaveChangesAsync();
        TempData["Success"] = "Видання оновлено";
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Publications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : View(entity);
    }
    [Authorize]
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await db.Publications.FindAsync(id);
        if (entity is null) return NotFound();
        if (await db.Copies.AnyAsync(x => x.PublicationId == id)) TempData["Error"] = "Спочатку вилучіть примірники видання";
        else
        {
            db.Publications.Remove(entity);
            try { await db.SaveChangesAsync(); TempData["Success"] = "Видання видалено"; }
            catch (DbUpdateException) { TempData["Error"] = "Не вдалося видалити пов’язане видання"; }
        }
        return RedirectToAction(nameof(Index));
    }
}
