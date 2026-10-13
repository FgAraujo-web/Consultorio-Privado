using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class HorarioMedicoFormViewModel
{
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "Selecione o dia da semana.")]
    [Range(0, 6, ErrorMessage = "Dia da semana inválido.")]
    public DayOfWeek? DiaSemana { get; set; }

    [Required(ErrorMessage = "Indique a hora de início.")]
    [DataType(DataType.Time)]
    public TimeSpan? HoraInicio { get; set; }

    [Required(ErrorMessage = "Indique a hora de fim.")]
    [DataType(DataType.Time)]
    public TimeSpan? HoraFim { get; set; }
}