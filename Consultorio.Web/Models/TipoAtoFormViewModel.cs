using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class TipoAtoFormViewModel
{
    public int TipoAtoId { get; set; }

    [Required(ErrorMessage = "Indique o código.")]
    [StringLength(30, ErrorMessage = "O código não pode exceder 30 caracteres.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = "";

    [Required(ErrorMessage = "Indique a designação.")]
    [StringLength(150, ErrorMessage = "A designação não pode exceder 150 caracteres.")]
    [Display(Name = "Designação")]
    public string Designacao { get; set; } = "";

    [Range(typeof(decimal), "0", "99999999", ErrorMessage = "O preço base deve ser igual ou superior a zero.")]
    [Display(Name = "Preço base (€)")]
    public decimal PrecoBase { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
