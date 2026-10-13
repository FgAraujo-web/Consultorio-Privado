using Consultorio.Domain.Entities;
using Consultorio.Infrastructure.Data;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class TiposAtosController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        q = q?.Trim();
        IQueryable<TipoAto> query = db.TiposAtos.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Codigo.Contains(q) || x.Designacao.Contains(q));

        var total = await query.CountAsync();
        const int pageSize = 25;
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Min(page, totalPages);

        ViewBag.Query = q;
        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Total = total;
        return View(await query.OrderBy(x => x.TipoAtoId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.TiposAtos.AsNoTracking().FirstOrDefaultAsync(x => x.TipoAtoId == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public IActionResult Create() => View(new TipoAtoFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TipoAtoFormViewModel model)
    {
        Normalizar(model);
        await ValidarCodigoUnico(model);
        if (!ModelState.IsValid) return View(model);

        var item = new TipoAto
        {
            Codigo = model.Codigo,
            Designacao = model.Designacao,
            PrecoBase = model.PrecoBase,
            Ativo = model.Ativo
        };
        db.TiposAtos.Add(item);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Codigo), "Não foi possível guardar. Verifique se o código já existe.");
            return View(model);
        }
        TempData["MensagemSucesso"] = "Tipo de ato registado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = item.TipoAtoId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.TiposAtos.AsNoTracking().FirstOrDefaultAsync(x => x.TipoAtoId == id);
        if (item is null) return NotFound();
        return View(new TipoAtoFormViewModel
        {
            TipoAtoId = item.TipoAtoId,
            Codigo = item.Codigo,
            Designacao = item.Designacao,
            PrecoBase = item.PrecoBase,
            Ativo = item.Ativo
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TipoAtoFormViewModel model)
    {
        if (id != model.TipoAtoId) return BadRequest();
        Normalizar(model);
        await ValidarCodigoUnico(model, id);
        if (!ModelState.IsValid) return View(model);

        var item = await db.TiposAtos.FindAsync(id);
        if (item is null) return NotFound();
        item.Codigo = model.Codigo;
        item.Designacao = model.Designacao;
        item.PrecoBase = model.PrecoBase;
        item.Ativo = model.Ativo;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Codigo), "Não foi possível guardar. Verifique se o código já existe.");
            return View(model);
        }
        TempData["MensagemSucesso"] = "Tipo de ato atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarEstado(int id)
    {
        var item = await db.TiposAtos.FindAsync(id);
        if (item is null) return NotFound();
        item.Ativo = !item.Ativo;
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = item.Ativo
            ? "Tipo de ato ativado com sucesso."
            : "Tipo de ato desativado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private static void Normalizar(TipoAtoFormViewModel model)
    {
        model.Codigo = (model.Codigo ?? "").Trim();
        model.Designacao = (model.Designacao ?? "").Trim();
    }

    private async Task ValidarCodigoUnico(TipoAtoFormViewModel model, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(model.Codigo)) return;
        var existe = await db.TiposAtos.AsNoTracking().AnyAsync(x =>
            x.Codigo == model.Codigo && (!excluirId.HasValue || x.TipoAtoId != excluirId.Value));
        if (existe) ModelState.AddModelError(nameof(model.Codigo), "Já existe um tipo de ato com este código.");
    }
}
