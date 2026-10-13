using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class AtoClinicoFormViewModel
{
    public int AtoId { get; set; }

    [Display(Name = "Consulta")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma consulta.")]
    public int ConsultaId { get; set; }

    [Display(Name = "Tipo de ato")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um tipo de ato.")]
    public int TipoAtoId { get; set; }

    [Display(Name = "Data do ato")]
    [Required(ErrorMessage = "Indique a data do ato.")]
    public DateTime DataAto { get; set; } = DateTime.Now;

    [Display(Name = "Descrição")]
    [StringLength(2000)]
    public string? Descricao { get; set; }

    [Display(Name = "Valor (€)")]
    [Range(typeof(decimal), "0", "99999999", ErrorMessage = "Indique um valor válido.")]
    public decimal Valor { get; set; }
}
