using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class CriarUtenteViewModel
{
    [Required(ErrorMessage = "Indique o código do utente.")]
    [StringLength(50, ErrorMessage = "O código não pode exceder 50 caracteres.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = "";

    [Required(ErrorMessage = "Indique o nome do utente.")]
    [StringLength(150, ErrorMessage = "O nome não pode exceder 150 caracteres.")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Indique a data de nascimento.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateTime? DataNascimento { get; set; }

    [Phone(ErrorMessage = "Indique um telefone válido.")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "Indique um email válido.")]
    public string? Email { get; set; }

    public string? Localidade { get; set; }
    public bool Ativo { get; set; } = true;
}
