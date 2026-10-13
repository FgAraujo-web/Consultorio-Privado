using Consultorio.Infrastructure.Data;
using Consultorio.Domain.Entities;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class UtentesController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        IQueryable<Utente> rows = db.Utentes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
            rows = rows.Where(x => x.Nome.Contains(q));
        ViewBag.Query = q;
        ViewBag.Page = page;
        return View(await rows.OrderBy(x => x.UtenteId)
            .Skip((page - 1) * 25).Take(25).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Utentes.FindAsync(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public IActionResult Create() => View(new CriarUtenteViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CriarUtenteViewModel model)
    {
        model.Codigo = model.Codigo?.Trim() ?? "";
        model.Nome = model.Nome?.Trim() ?? "";
        model.Telefone = string.IsNullOrWhiteSpace(model.Telefone) ? null : model.Telefone.Trim();
        model.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        model.Localidade = string.IsNullOrWhiteSpace(model.Localidade) ? null : model.Localidade.Trim();

        if (model.DataNascimento.HasValue && model.DataNascimento.Value.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.DataNascimento), "A data de nascimento não pode ser futura.");

        if (await db.Utentes.AnyAsync(x => x.Codigo == model.Codigo))
            ModelState.AddModelError(nameof(model.Codigo), "Já existe um utente com este código.");

        if (!ModelState.IsValid)
            return View(model);

        var utente = new Utente
        {
            Codigo = model.Codigo,
            Nome = model.Nome,
            DataNascimento = model.DataNascimento!.Value.Date,
            Telefone = model.Telefone,
            Email = model.Email,
            Localidade = model.Localidade,
            Ativo = model.Ativo
        };

        db.Utentes.Add(utente);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Não foi possível guardar o utente. Verifique se o código já existe e tente novamente.");
            return View(model);
        }

        TempData["MensagemSucesso"] = "Utente criado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = utente.UtenteId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var utente = await db.Utentes.AsNoTracking().FirstOrDefaultAsync(x => x.UtenteId == id);
        if (utente is null) return NotFound();

        var model = new CriarUtenteViewModel
        {
            Codigo = utente.Codigo,
            Nome = utente.Nome,
            DataNascimento = utente.DataNascimento,
            Telefone = utente.Telefone,
            Email = utente.Email,
            Localidade = utente.Localidade,
            Ativo = utente.Ativo
        };
        ViewBag.UtenteId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CriarUtenteViewModel model)
    {
        var utente = await db.Utentes.FindAsync(id);
        if (utente is null) return NotFound();

        model.Codigo = model.Codigo?.Trim() ?? "";
        model.Nome = model.Nome?.Trim() ?? "";
        model.Telefone = string.IsNullOrWhiteSpace(model.Telefone) ? null : model.Telefone.Trim();
        model.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        model.Localidade = string.IsNullOrWhiteSpace(model.Localidade) ? null : model.Localidade.Trim();

        if (model.DataNascimento.HasValue && model.DataNascimento.Value.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.DataNascimento), "A data de nascimento não pode ser futura.");

        if (await db.Utentes.AnyAsync(x => x.Codigo == model.Codigo && x.UtenteId != id))
            ModelState.AddModelError(nameof(model.Codigo), "Já existe outro utente com este código.");

        if (!ModelState.IsValid)
        {
            ViewBag.UtenteId = id;
            return View(model);
        }

        utente.Codigo = model.Codigo;
        utente.Nome = model.Nome;
        utente.DataNascimento = model.DataNascimento!.Value.Date;
        utente.Telefone = model.Telefone;
        utente.Email = model.Email;
        utente.Localidade = model.Localidade;
        utente.Ativo = model.Ativo;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Não foi possível guardar as alterações. Verifique se o código já existe.");
            ViewBag.UtenteId = id;
            return View(model);
        }

        TempData["MensagemSucesso"] = "Utente atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarEstado(int id)
    {
        var utente = await db.Utentes.FindAsync(id);

        if (utente == null)
            return NotFound();

        utente.Ativo = !utente.Ativo;

        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] = utente.Ativo
            ? "Utente reativado com sucesso."
            : "Utente desativado com sucesso.";

        return RedirectToAction(nameof(Details), new { id });
    }


}
