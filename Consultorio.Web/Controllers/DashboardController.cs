
using Consultorio.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ConsultorioDbContext _context;

    public DashboardController(ConsultorioDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Indicadores gerais
        ViewBag.TotalUtentes =
            await _context.Utentes.CountAsync();

        ViewBag.TotalMedicos =
            await _context.Medicos.CountAsync();

        ViewBag.TotalConsultas =
            await _context.Consultas.CountAsync();

        ViewBag.TotalPagamentos =
            await _context.Pagamentos.CountAsync();

        ViewBag.TotalAtosClinicos =
            await _context.AtosClinicos.CountAsync();

        // Total recebido
        ViewBag.ValorRecebido =
            await _context.Pagamentos
                .SumAsync(p => (decimal?)p.Valor) ?? 0m;

        // Consultas agrupadas por estado
        ViewBag.ConsultasPorEstado =
            await _context.Consultas
                .AsNoTracking()
                .GroupBy(c => c.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

        // Próximas consultas agendadas
        var agora = DateTime.Now;

        ViewBag.ProximasConsultas =
            await _context.Consultas
                .AsNoTracking()
                .Where(c => c.DataHora >= agora
                         && c.Estado == "Agendada")
                .OrderBy(c => c.DataHora)
                .Select(c => new
                {
                    c.ConsultaId,
                    c.DataHora,
                    Utente = c.Utente != null
                        ? c.Utente.Nome : "—",
                    Medico = c.Medico != null
                        ? c.Medico.Nome : "—"
                })
                .Take(10)
                .ToListAsync();

        return View();
    }
}
