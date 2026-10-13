using Consultorio.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Consultorio.Web.Models;

public class AgendaSemanalViewModel
{
    public DateTime InicioSemana { get; set; }
    public int? MedicoId { get; set; }
    public List<SelectListItem> Medicos { get; set; } = new();
    public List<Consulta> Consultas { get; set; } = new();
}
