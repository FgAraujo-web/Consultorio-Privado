using System;
using System.Collections.Generic;
using System.Text;

namespace Consultorio.Domain.Entities;

public class HorarioMedico
{
    public int HorarioMedicoId { get; set; }

    public int MedicoId { get; set; }
    public Medico? Medico { get; set; }

    public DayOfWeek DiaSemana { get; set; }

    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }

    public bool Ativo { get; set; } = true;
}