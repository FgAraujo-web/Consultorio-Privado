using Consultorio.Domain.Entities;
using Consultorio.Infrastructure.Data;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class AtosClinicosController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        var rows = db.AtosClinicos.AsNoTracking()
            .Include(x => x.TipoAto)
            .Include(x => x.Consulta).ThenInclude(c => c!.Utente)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            rows = rows.Where(x =>
                (x.Descricao != null && x.Descricao.Contains(q)) ||
                (x.TipoAto != null && x.TipoAto.Codigo.Contains(q)) ||
                (x.Consulta != null && x.Consulta.Utente != null && x.Consulta.Utente.Nome.Contains(q)));
        }
        ViewBag.Query = q;
        ViewBag.Page = page;
        ViewBag.HasNext = await rows.Skip(page * 25).AnyAsync();
        return View(await rows.OrderByDescending(x => x.DataAto).ThenByDescending(x => x.AtoId)
            .Skip((page - 1) * 25).Take(25).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.AtosClinicos.AsNoTracking()
            .Include(x => x.TipoAto)
            .Include(x => x.Consulta).ThenInclude(c => c!.Utente)
            .Include(x => x.Consulta).ThenInclude(c => c!.Medico)
            .FirstOrDefaultAsync(x => x.AtoId == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? consultaId)
    {
        await CarregarListasAsync();
        return View(new AtoClinicoFormViewModel { ConsultaId = consultaId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AtoClinicoFormViewModel model)
    {
        await ValidarAsync(model);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return View(model);
        }
        var item = new AtoClinico
        {
            ConsultaId = model.ConsultaId,
            TipoAtoId = model.TipoAtoId,
            DataAto = model.DataAto,
            Descricao = model.Descricao?.Trim(),
            Valor = model.Valor,
            Anulado = false
        };
        db.AtosClinicos.Add(item);
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Ato clínico registado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = item.AtoId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.AtosClinicos.AsNoTracking().FirstOrDefaultAsync(x => x.AtoId == id);
        if (item is null) return NotFound();
        if (item.Anulado) return BadRequest("Não é possível editar um ato anulado.");
        await CarregarListasAsync(item.ConsultaId);
        return View(new AtoClinicoFormViewModel
        {
            AtoId = item.AtoId,
            ConsultaId = item.ConsultaId,
            TipoAtoId = item.TipoAtoId,
            DataAto = item.DataAto,
            Descricao = item.Descricao,
            Valor = item.Valor
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AtoClinicoFormViewModel model)
    {
        if (id != model.AtoId) return BadRequest();
        var item = await db.AtosClinicos.FindAsync(id);
        if (item is null) return NotFound();
        if (item.Anulado) return BadRequest("Não é possível editar um ato anulado.");
        await ValidarAsync(model);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync(item.ConsultaId);
            return View(model);
        }
        item.ConsultaId = model.ConsultaId;
        item.TipoAtoId = model.TipoAtoId;
        item.DataAto = model.DataAto;
        item.Descricao = model.Descricao?.Trim();
        item.Valor = model.Valor;
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Ato clínico atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id)
    {
        var item = await db.AtosClinicos.FindAsync(id);
        if (item is null) return NotFound();
        if (!item.Anulado)
        {
            item.Anulado = true;
            await db.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Ato clínico anulado com sucesso.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task ValidarAsync(AtoClinicoFormViewModel model)
    {
        if (!await db.Consultas.AnyAsync(c => c.ConsultaId == model.ConsultaId && c.Estado == "Realizada"))
            ModelState.AddModelError(nameof(model.ConsultaId), "Selecione uma consulta realizada.");
        if (!await db.TiposAtos.AnyAsync(t => t.TipoAtoId == model.TipoAtoId))
            ModelState.AddModelError(nameof(model.TipoAtoId), "Selecione um tipo de ato válido.");
        if (model.DataAto > DateTime.Now)
            ModelState.AddModelError(nameof(model.DataAto), "A data do ato não pode ser futura.");
    }

    private async Task CarregarListasAsync(int? consultaAtualId = null)
    {
        var consultas = await db.Consultas.AsNoTracking()
            .Where(c => c.Estado == "Realizada" || (consultaAtualId.HasValue && c.ConsultaId == consultaAtualId.Value))
            .OrderByDescending(c => c.DataHora)
            .Select(c => new { c.ConsultaId, c.DataHora, Utente = c.Utente != null ? c.Utente.Nome : "—" })
            .ToListAsync();
        ViewBag.Consultas = new SelectList(consultas.Select(c => new
        {
            c.ConsultaId,
            Texto = $"#{c.ConsultaId} · {c.DataHora:dd/MM/yyyy HH:mm} · {c.Utente}"
        }), "ConsultaId", "Texto");
        var tipos = await db.TiposAtos.AsNoTracking().OrderBy(t => t.Codigo)
            .Select(t => new { t.TipoAtoId, t.Codigo }).ToListAsync();
        ViewBag.TiposAtos = new SelectList(tipos, "TipoAtoId", "Codigo");
    }
}
