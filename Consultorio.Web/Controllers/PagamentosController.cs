using Consultorio.Domain.Entities;
using Consultorio.Infrastructure.Data;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class PagamentosController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        IQueryable<Pagamento> rows = db.Pagamentos.AsNoTracking()
            .Include(p => p.Consulta).ThenInclude(c => c!.Utente);
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            rows = rows.Where(p => p.ReferenciaFicticia.Contains(q) ||
                p.Metodo.Contains(q) ||
                (p.Consulta != null && p.Consulta.Utente != null && p.Consulta.Utente.Nome.Contains(q)));
        }
        ViewBag.Query = q;
        ViewBag.Page = page;
        ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(await rows.CountAsync() / 25.0));
        return View(await rows.OrderByDescending(p => p.DataPagamento)
            .ThenByDescending(p => p.PagamentoId).Skip((page - 1) * 25).Take(25).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var pagamento = await db.Pagamentos.AsNoTracking()
            .Include(p => p.Consulta).ThenInclude(c => c!.Utente)
            .Include(p => p.Consulta).ThenInclude(c => c!.Medico)
            .FirstOrDefaultAsync(p => p.PagamentoId == id);
        return pagamento is null ? NotFound() : View(pagamento);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? consultaId)
    {
        await CarregarConsultasAsync();
        return View(new PagamentoFormViewModel { ConsultaId = consultaId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagamentoFormViewModel model)
    {
        await ValidarAsync(model);
        if (!ModelState.IsValid)
        {
            await CarregarConsultasAsync();
            return View(model);
        }
        var pagamento = new Pagamento
        {
            ConsultaId = model.ConsultaId,
            Valor = model.Valor,
            DataPagamento = model.DataPagamento,
            Metodo = model.Metodo.Trim(),
            ReferenciaFicticia = model.ReferenciaFicticia.Trim()
        };
        db.Pagamentos.Add(pagamento);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.ReferenciaFicticia), "Não foi possível guardar. Confirme que a referência é única.");
            await CarregarConsultasAsync();
            return View(model);
        }
        TempData["MensagemSucesso"] = "Pagamento registado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = pagamento.PagamentoId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await db.Pagamentos.FindAsync(id);
        if (p is null) return NotFound();
        await CarregarConsultasAsync();
        return View(new PagamentoFormViewModel
        {
            PagamentoId = p.PagamentoId, ConsultaId = p.ConsultaId,
            Valor = p.Valor, DataPagamento = p.DataPagamento,
            Metodo = p.Metodo, ReferenciaFicticia = p.ReferenciaFicticia
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PagamentoFormViewModel model)
    {
        if (id != model.PagamentoId) return BadRequest();
        var pagamento = await db.Pagamentos.FindAsync(id);
        if (pagamento is null) return NotFound();
        await ValidarAsync(model, id);
        if (!ModelState.IsValid)
        {
            await CarregarConsultasAsync();
            return View(model);
        }
        pagamento.ConsultaId = model.ConsultaId;
        pagamento.Valor = model.Valor;
        pagamento.DataPagamento = model.DataPagamento;
        pagamento.Metodo = model.Metodo.Trim();
        pagamento.ReferenciaFicticia = model.ReferenciaFicticia.Trim();
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.ReferenciaFicticia), "Não foi possível guardar. Confirme que a referência é única.");
            await CarregarConsultasAsync();
            return View(model);
        }
        TempData["MensagemSucesso"] = "Pagamento atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task ValidarAsync(PagamentoFormViewModel model, int? id = null)
    {
        if (!await db.Consultas.AnyAsync(c => c.ConsultaId == model.ConsultaId))
            ModelState.AddModelError(nameof(model.ConsultaId), "A consulta selecionada não existe.");
        if (model.DataPagamento > DateTime.Now)
            ModelState.AddModelError(nameof(model.DataPagamento), "A data do pagamento não pode ser futura.");
        var referencia = model.ReferenciaFicticia?.Trim();
        if (!string.IsNullOrEmpty(referencia) && await db.Pagamentos.AnyAsync(p =>
            p.ReferenciaFicticia == referencia && (!id.HasValue || p.PagamentoId != id.Value)))
            ModelState.AddModelError(nameof(model.ReferenciaFicticia), "Esta referência já está registada.");
    }

    private async Task CarregarConsultasAsync()
    {
        var consultas = await db.Consultas.AsNoTracking()
            .Include(c => c.Utente)
            .OrderByDescending(c => c.DataHora)
            .Select(c => new { c.ConsultaId, c.DataHora, Utente = c.Utente != null ? c.Utente.Nome : "—" })
            .ToListAsync();
        ViewBag.Consultas = new SelectList(consultas.Select(c => new
        {
            c.ConsultaId,
            Texto = $"#{c.ConsultaId} · {c.DataHora:dd/MM/yyyy HH:mm} · {c.Utente}"
        }), "ConsultaId", "Texto");
    }
}
