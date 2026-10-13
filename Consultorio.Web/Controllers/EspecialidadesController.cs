using Consultorio.Infrastructure.Data;
using Consultorio.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Consultorio.Web.Controllers;
public class EspecialidadesController(ConsultorioDbContext db) : Controller {
 public async Task<IActionResult> Index(string? q,int page=1) {
  page=Math.Max(1,page);
  IQueryable<Especialidade> rows=db.Especialidades.AsNoTracking();
  if (!string.IsNullOrWhiteSpace(q)) rows = rows.Where(x=>x.Designacao.Contains(q));
  ViewBag.Query=q; ViewBag.Page=page;
  return View(await rows.OrderBy(x=>x.EspecialidadeId).Skip((page-1)*25).Take(25).ToListAsync());
 }
 public async Task<IActionResult> Details(int id) { var item=await db.Especialidades.FindAsync(id); return item is null?NotFound():View(item); }
}
