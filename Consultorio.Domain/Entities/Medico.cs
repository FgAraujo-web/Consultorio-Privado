namespace Consultorio.Domain.Entities;
public class Medico { public int MedicoId { get; set; } public string Nome { get; set; } = ""; public string CedulaFicticia { get; set; } = ""; public int EspecialidadeId { get; set; } public Especialidade? Especialidade { get; set; } public bool Ativo { get; set; } = true; }
