using System;
using System.Collections.Generic;
using System.Text;

namespace Consultorio.Domain.Entities;

public class IndisponibilidadeMedico
{
    public int IndisponibilidadeMedicoId { get; set; }

    public int MedicoId { get; set; }
    public Medico? Medico { get; set; }

    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }

    public string? Motivo { get; set; }
}