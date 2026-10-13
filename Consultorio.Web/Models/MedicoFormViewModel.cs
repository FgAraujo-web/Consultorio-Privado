using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class MedicoFormViewModel
{
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "Indique o nome do médico.")]
    [StringLength(150)]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Indique a cédula fictícia.")]
    [StringLength(50)]
    [Display(Name = "Cédula fictícia")]
    public string CedulaFicticia { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma especialidade.")]
    [Display(Name = "Especialidade")]
    public int EspecialidadeId { get; set; }

    public bool Ativo { get; set; } = true;
}
