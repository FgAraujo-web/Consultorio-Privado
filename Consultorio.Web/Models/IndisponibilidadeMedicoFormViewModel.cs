using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class IndisponibilidadeMedicoFormViewModel
{
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "Indique a data e hora de início.")]
    [Display(Name = "Data e hora de início")]
    [DataType(DataType.DateTime)]
    public DateTime? DataHoraInicio { get; set; }

    [Required(ErrorMessage = "Indique a data e hora de fim.")]
    [Display(Name = "Data e hora de fim")]
    [DataType(DataType.DateTime)]
    public DateTime? DataHoraFim { get; set; }

    [StringLength(250, ErrorMessage = "O motivo não pode ultrapassar 250 caracteres.")]
    [Display(Name = "Motivo")]
    public string? Motivo { get; set; }
}