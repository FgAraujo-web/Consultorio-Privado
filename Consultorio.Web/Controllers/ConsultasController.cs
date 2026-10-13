using Consultorio.Domain.Entities;
using Consultorio.Infrastructure.Data;
using Consultorio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class ConsultasController(ConsultorioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        IQueryable<Consulta> rows = db.Consultas.AsNoTracking()
            .Include(c => c.Utente).Include(c => c.Medico);
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            rows = rows.Where(c => (c.Utente != null && c.Utente.Nome.Contains(q)) ||
                                   (c.Medico != null && c.Medico.Nome.Contains(q)) ||
                                   c.Estado.Contains(q));
        }
        ViewBag.Query = q;
        ViewBag.Page = page;
        return View(await rows.OrderByDescending(c => c.DataHora)
            .ThenByDescending(c => c.ConsultaId)
            .Skip((page - 1) * 25).Take(25).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var consulta = await db.Consultas.AsNoTracking()
            .Include(c => c.Utente).Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.ConsultaId == id);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CarregarListasAsync();
        return View(new ConsultaFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ConsultaFormViewModel model)
    {
        await ValidarAgendamentoAsync(model);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync(model.UtenteId, model.MedicoId);
            return View(model);
        }

        var consulta = new Consulta
        {
            UtenteId = model.UtenteId,
            MedicoId = model.MedicoId,
            DataHora = model.DataHora!.Value,
            DuracaoMinutos = model.DuracaoMinutos,
            Estado = "Agendada",
            Observacoes = model.Observacoes?.Trim()
        };
        db.Consultas.Add(consulta);
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Consulta agendada com sucesso.";
        return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var consulta = await db.Consultas.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ConsultaId == id);
        if (consulta is null) return NotFound();
        if (consulta.Estado != "Agendada")
        {
            TempData["MensagemErro"] = "Apenas consultas agendadas podem ser editadas.";
            return RedirectToAction(nameof(Details), new { id });
        }
        await CarregarListasAsync(consulta.UtenteId, consulta.MedicoId);
        return View(new ConsultaFormViewModel
        {
            ConsultaId = consulta.ConsultaId,
            UtenteId = consulta.UtenteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora,
            DuracaoMinutos = consulta.DuracaoMinutos,
            Observacoes = consulta.Observacoes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ConsultaFormViewModel model)
    {
        if (id != model.ConsultaId) return BadRequest();
        var consulta = await db.Consultas.FirstOrDefaultAsync(c => c.ConsultaId == id);
        if (consulta is null) return NotFound();
        if (consulta.Estado != "Agendada") return BadRequest("A consulta já não está agendada.");
        await ValidarAgendamentoAsync(model, id);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync(model.UtenteId, model.MedicoId);
            return View(model);
        }
        consulta.UtenteId = model.UtenteId;
        consulta.MedicoId = model.MedicoId;
        consulta.DataHora = model.DataHora!.Value;
        consulta.DuracaoMinutos = model.DuracaoMinutos;
        consulta.Observacoes = model.Observacoes?.Trim();
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Consulta atualizada com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarEstado(int id, string novoEstado)
    {
        if (novoEstado != "Realizada" && novoEstado != "Cancelada") return BadRequest();
        var consulta = await db.Consultas.FindAsync(id);
        if (consulta is null) return NotFound();
        if (consulta.Estado != "Agendada")
        {
            TempData["MensagemErro"] = "Só é possível alterar o estado de consultas agendadas.";
            return RedirectToAction(nameof(Details), new { id });
        }
        consulta.Estado = novoEstado;
        await db.SaveChangesAsync();
        TempData["MensagemSucesso"] = novoEstado == "Realizada"
            ? "Consulta marcada como realizada." : "Consulta cancelada com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task CarregarListasAsync(int? utenteId = null, int? medicoId = null)
    {
        var utentes = await db.Utentes.AsNoTracking()
            .Where(u => u.Ativo || u.UtenteId == utenteId)
            .OrderBy(u => u.Nome)
            .Select(u => new { u.UtenteId, u.Nome, u.Codigo }).ToListAsync();
        var medicos = await db.Medicos.AsNoTracking()
            .Where(m => m.Ativo || m.MedicoId == medicoId)
            .OrderBy(m => m.Nome)
            .Select(m => new { m.MedicoId, m.Nome }).ToListAsync();
        ViewBag.Utentes = new SelectList(utentes.Select(u => new
        {
            u.UtenteId, Texto = $"{u.Nome} ({u.Codigo})"
        }), "UtenteId", "Texto", utenteId);
        ViewBag.Medicos = new SelectList(medicos, "MedicoId", "Nome", medicoId);
    }

    private async Task ValidarAgendamentoAsync(
      ConsultaFormViewModel model,
      int? excluirConsultaId = null)
    {
        if (model.UtenteId > 0 &&
            !await db.Utentes.AnyAsync(u =>
                u.UtenteId == model.UtenteId && u.Ativo))
        {
            ModelState.AddModelError(
                nameof(model.UtenteId),
                "Selecione um utente ativo.");
        }

        if (model.MedicoId > 0 &&
            !await db.Medicos.AnyAsync(m =>
                m.MedicoId == model.MedicoId && m.Ativo))
        {
            ModelState.AddModelError(
                nameof(model.MedicoId),
                "Selecione um médico ativo.");
        }

        if (model.DataHora is not { } data)
            return;

        if (data < DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.DataHora),
                "A data e hora não podem ser anteriores ao momento atual.");
        }

        if (model.DuracaoMinutos is not (15 or 30 or 45 or 60))
        {
            ModelState.AddModelError(
                nameof(model.DuracaoMinutos),
                "Escolha 15, 30, 45 ou 60 minutos.");

            return;
        }

        if (model.MedicoId <= 0)
            return;

        var fim = data.AddMinutes(model.DuracaoMinutos);

        // REGRA 1 — Horários semanais do médico

        var horarios = await db.HorariosMedicos
            .AsNoTracking()
            .Where(h => h.MedicoId == model.MedicoId && h.Ativo)
            .ToListAsync();

        if (horarios.Count > 0)
        {
            // A consulta deve caber integralmente num período
            // de atendimento do dia correspondente.

            bool dentroHorario = data.Date == fim.AddTicks(-1).Date &&
                horarios.Any(h =>
                    h.DiaSemana == data.DayOfWeek &&
                    data.TimeOfDay >= h.HoraInicio &&
                    fim.TimeOfDay <= h.HoraFim);

            if (!dentroHorario)
            {
                ModelState.AddModelError(
                    nameof(model.DataHora),
                    "A consulta está fora do horário de atendimento do médico.");
            }
        }

        // REGRA 2 — Indisponibilidades do médico

        bool indisponivel = await db.IndisponibilidadesMedicos
            .AsNoTracking()
            .AnyAsync(i =>
                i.MedicoId == model.MedicoId &&
                i.DataHoraInicio < fim &&
                data < i.DataHoraFim);

        if (indisponivel)
        {
            ModelState.AddModelError(
                nameof(model.DataHora),
                "O médico encontra-se indisponível no período selecionado.");
        }

        // REGRA 3 — Sobreposição com outras consultas
        // Preserva a validação implementada na Fase 10.2.

        var inicioPesquisa = data.AddMinutes(-60);

        var ocupadas = await db.Consultas
            .AsNoTracking()
            .Where(c =>
                c.MedicoId == model.MedicoId &&
                c.Estado == "Agendada" &&
                c.DataHora < fim &&
                c.DataHora >= inicioPesquisa &&
                (!excluirConsultaId.HasValue ||
                 c.ConsultaId != excluirConsultaId.Value))
            .Select(c => new
            {
                c.DataHora,
                c.DuracaoMinutos
            })
            .ToListAsync();

        bool sobreposicao = ocupadas.Any(c =>
            c.DataHora.AddMinutes(c.DuracaoMinutos) > data);

        if (sobreposicao)
        {
            ModelState.AddModelError(
                nameof(model.DataHora),
                "O médico já tem uma consulta agendada que se sobrepõe a este horário.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Agenda(DateTime? semana, int? medicoId)
    {
        // A semana começa sempre à segunda-feira.
        var referencia = (semana ?? DateTime.Today).Date;
        var inicio = referencia.AddDays(-((int)referencia.DayOfWeek + 6) % 7);
        var fimExclusivo = inicio.AddDays(7);

        var medicos = await db.Medicos.AsNoTracking()
            .OrderBy(m => m.Nome)
            .Select(m => new { m.MedicoId, m.Nome })
            .ToListAsync();

        var consultasQuery = db.Consultas.AsNoTracking()
            .Include(c => c.Utente)
            .Include(c => c.Medico)
            .Where(c => c.DataHora >= inicio && c.DataHora < fimExclusivo);

        if (medicoId.HasValue && medicoId.Value > 0)
            consultasQuery = consultasQuery.Where(c => c.MedicoId == medicoId.Value);

        var consultas = await consultasQuery
            .OrderBy(c => c.DataHora)
            .ThenBy(c => c.ConsultaId)
            .ToListAsync();

        return View(new AgendaSemanalViewModel
        {
            InicioSemana = inicio,
            MedicoId = medicoId,
            Medicos = medicos.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = m.MedicoId.ToString(),
                Text = m.Nome
            }).ToList(),
            Consultas = consultas
        });
    }

}
