using Consultorio.Domain.Entities;
using Consultorio.Infrastructure.Data;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class MedicosController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        IQueryable<Medico> rows = db.Medicos.AsNoTracking().Include(m => m.Especialidade);
        if (!string.IsNullOrWhiteSpace(q))
            rows = rows.Where(x => x.Nome.Contains(q));
        ViewBag.Query = q;
        ViewBag.Page = page;
        return View(await rows.OrderBy(x => x.MedicoId).Skip((page - 1) * 25).Take(25).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Medicos.AsNoTracking().Include(m => m.Especialidade)
            .FirstOrDefaultAsync(m => m.MedicoId == id);
        return item is null ? NotFound() : View(item);
    }


    private async Task CarregarEspecialidadesAsync()
    {
        var especialidades = await db.Especialidades
            .AsNoTracking()
            .OrderBy(e => e.Designacao)
            .Select(e => new
            {
                e.EspecialidadeId,
                e.Designacao
            })
            .ToListAsync();

        ViewBag.Especialidades = new SelectList(
            especialidades,
            "EspecialidadeId",
            "Designacao"
        );
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CarregarEspecialidadesAsync();
        return View(new MedicoFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicoFormViewModel model)
    {
        await ValidarDadosAsync(model);
        if (!ModelState.IsValid)
        {
            await CarregarEspecialidadesAsync();
            return View(model);
        }

        var medico = new Medico
        {
            Nome = model.Nome.Trim(),
            CedulaFicticia = model.CedulaFicticia.Trim(),
            EspecialidadeId = model.EspecialidadeId,
            Ativo = model.Ativo
        };
        db.Medicos.Add(medico);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.CedulaFicticia),
                "Não foi possível guardar. Verifique se a cédula fictícia já existe.");
            await CarregarEspecialidadesAsync();
            return View(model);
        }
        TempData["MensagemSucesso"] = "Médico criado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = medico.MedicoId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var medico = await db.Medicos.AsNoTracking().FirstOrDefaultAsync(x => x.MedicoId == id);
        if (medico is null) return NotFound();
        await CarregarEspecialidadesAsync();
        return View(new MedicoFormViewModel
        {
            MedicoId = medico.MedicoId,
            Nome = medico.Nome,
            CedulaFicticia = medico.CedulaFicticia,
            EspecialidadeId = medico.EspecialidadeId,
            Ativo = medico.Ativo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MedicoFormViewModel model)
    {
        if (id != model.MedicoId) return BadRequest();
        var medico = await db.Medicos.FindAsync(id);
        if (medico is null) return NotFound();
        await ValidarDadosAsync(model, id);
        if (!ModelState.IsValid)
        {
            await CarregarEspecialidadesAsync();
            return View(model);
        }
        medico.Nome = model.Nome.Trim();
        medico.CedulaFicticia = model.CedulaFicticia.Trim();
        medico.EspecialidadeId = model.EspecialidadeId;
        medico.Ativo = model.Ativo;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.CedulaFicticia),
                "Não foi possível guardar. Verifique se a cédula fictícia já existe.");
            await CarregarEspecialidadesAsync();
            return View(model);
        }
        TempData["MensagemSucesso"] = "Médico atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task ValidarDadosAsync(MedicoFormViewModel model, int? excluirId = null)
    {
        if (!string.IsNullOrWhiteSpace(model.CedulaFicticia))
        {
            var cedula = model.CedulaFicticia.Trim();
            if (await db.Medicos.AnyAsync(m => m.CedulaFicticia == cedula &&
                (!excluirId.HasValue || m.MedicoId != excluirId.Value)))
                ModelState.AddModelError(nameof(model.CedulaFicticia), "Esta cédula fictícia já está registada.");
        }
        if (model.EspecialidadeId > 0 &&
            !await db.Especialidades.AnyAsync(e => e.EspecialidadeId == model.EspecialidadeId))
            ModelState.AddModelError(nameof(model.EspecialidadeId), "A especialidade selecionada não existe.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarEstado(int id)
    {
        var medico = await db.Medicos.FindAsync(id);
        if (medico is null) return NotFound();
        medico.Ativo = !medico.Ativo;
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = medico.Ativo
            ? "Médico reativado com sucesso." : "Médico desativado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Horarios(int id)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == id);

        if (medico is null)
            return NotFound();

        var horarios = await db.HorariosMedicos.AsNoTracking()
            .Where(h => h.MedicoId == id)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync();

        ViewBag.Medico = medico;

        return View(horarios);
    }

    [HttpGet]
    public async Task<IActionResult> CriarHorario(int id)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == id);

        if (medico is null)
            return NotFound();

        ViewBag.NomeMedico = medico.Nome;

        return View(new HorarioMedicoFormViewModel
        {
            MedicoId = id
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarHorario(
    HorarioMedicoFormViewModel model)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == model.MedicoId);

        if (medico is null)
            return NotFound();

        if (!medico.Ativo)
        {
            ModelState.AddModelError(
                "",
                "Não é possível adicionar horários a um médico inativo.");
        }

        if (model.HoraInicio.HasValue &&
            model.HoraFim.HasValue &&
            model.HoraFim <= model.HoraInicio)
        {
            ModelState.AddModelError(
                nameof(model.HoraFim),
                "A hora de fim deve ser posterior à hora de início.");
        }

        if (model.DiaSemana.HasValue &&
            model.HoraInicio.HasValue &&
            model.HoraFim.HasValue &&
            model.HoraFim > model.HoraInicio)
        {
            var horariosExistentes = await db.HorariosMedicos
                .AsNoTracking()
                .Where(h =>
                    h.MedicoId == model.MedicoId &&
                    h.DiaSemana == model.DiaSemana.Value &&
                    h.Ativo)
                .ToListAsync();

            bool sobreposicao = horariosExistentes.Any(h =>
                model.HoraInicio.Value < h.HoraFim &&
                h.HoraInicio < model.HoraFim.Value);

            if (sobreposicao)
            {
                ModelState.AddModelError(
                    nameof(model.HoraInicio),
                    "Já existe um horário que se sobrepõe a este período.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.NomeMedico = medico.Nome;
            return View(model);
        }

        db.HorariosMedicos.Add(new HorarioMedico
        {
            MedicoId = model.MedicoId,
            DiaSemana = model.DiaSemana!.Value,
            HoraInicio = model.HoraInicio!.Value,
            HoraFim = model.HoraFim!.Value,
            Ativo = true
        });

        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] =
            "Horário criado com sucesso.";

        return RedirectToAction(
            nameof(Horarios),
            new { id = model.MedicoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverHorario(
    int id,
    int medicoId)
    {
        var horario = await db.HorariosMedicos
            .FirstOrDefaultAsync(h =>
                h.HorarioMedicoId == id &&
                h.MedicoId == medicoId);

        if (horario is null)
            return NotFound();

        db.HorariosMedicos.Remove(horario);

        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] =
            "Horário removido com sucesso.";

        return RedirectToAction(
            nameof(Horarios),
            new { id = medicoId });
    }

    // Fase 10.3.2 — Listar indisponibilidades
    [HttpGet]
    public async Task<IActionResult> Indisponibilidades(int id)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == id);

        if (medico is null)
            return NotFound();

        var indisponibilidades = await db.IndisponibilidadesMedicos
            .AsNoTracking()
            .Where(i => i.MedicoId == id)
            .OrderBy(i => i.DataHoraInicio)
            .ToListAsync();

        ViewBag.Medico = medico;

        return View(indisponibilidades);
    }

    // Fase 10.3.2 — Abrir formulário
    [HttpGet]
    public async Task<IActionResult> CriarIndisponibilidade(int id)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == id);

        if (medico is null)
            return NotFound();

        ViewBag.NomeMedico = medico.Nome;

        return View(new IndisponibilidadeMedicoFormViewModel
        {
            MedicoId = id
        });
    }

    // Fase 10.3.2 — Guardar indisponibilidade
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarIndisponibilidade(
        IndisponibilidadeMedicoFormViewModel model)
    {
        var medico = await db.Medicos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MedicoId == model.MedicoId);

        if (medico is null)
            return NotFound();

        if (!medico.Ativo)
        {
            ModelState.AddModelError(
                "",
                "Não é possível adicionar indisponibilidades a um médico inativo.");
        }

        if (model.DataHoraInicio.HasValue &&
            model.DataHoraFim.HasValue)
        {
            var inicio = model.DataHoraInicio.Value;
            var fim = model.DataHoraFim.Value;

            if (fim <= inicio)
            {
                ModelState.AddModelError(
                    nameof(model.DataHoraFim),
                    "A data e hora de fim devem ser posteriores ao início.");
            }
            else
            {
                bool sobreposicao = await db.IndisponibilidadesMedicos
                    .AsNoTracking()
                    .AnyAsync(i =>
                        i.MedicoId == model.MedicoId &&
                        i.DataHoraInicio < fim &&
                        inicio < i.DataHoraFim);

                if (sobreposicao)
                {
                    ModelState.AddModelError(
                        nameof(model.DataHoraInicio),
                        "Já existe uma indisponibilidade que se sobrepõe a este período.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.NomeMedico = medico.Nome;
            return View(model);
        }

        db.IndisponibilidadesMedicos.Add(new IndisponibilidadeMedico
        {
            MedicoId = model.MedicoId,
            DataHoraInicio = model.DataHoraInicio!.Value,
            DataHoraFim = model.DataHoraFim!.Value,
            Motivo = model.Motivo?.Trim()
        });

        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] =
            "Indisponibilidade registada com sucesso.";

        return RedirectToAction(
            nameof(Indisponibilidades),
            new { id = model.MedicoId });
    }

    // Fase 10.3.2 — Remover indisponibilidade
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverIndisponibilidade(
        int id,
        int medicoId)
    {
        var indisponibilidade = await db.IndisponibilidadesMedicos
            .FirstOrDefaultAsync(i =>
                i.IndisponibilidadeMedicoId == id &&
                i.MedicoId == medicoId);

        if (indisponibilidade is null)
            return NotFound();

        db.IndisponibilidadesMedicos.Remove(indisponibilidade);

        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] =
            "Indisponibilidade removida com sucesso.";

        return RedirectToAction(
            nameof(Indisponibilidades),
            new { id = medicoId });
    }





}
