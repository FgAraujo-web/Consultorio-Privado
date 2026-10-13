namespace Consultorio.Domain.Entities;

public class Consulta
{
    public int ConsultaId { get; set; }
    public int UtenteId { get; set; }
    public Utente? Utente { get; set; }
    public int MedicoId { get; set; }
    public Medico? Medico { get; set; }
    public DateTime DataHora { get; set; }
    public int DuracaoMinutos { get; set; } = 30;
    public string Estado { get; set; } = "Agendada";
    public string? Observacoes { get; set; }
}
