namespace Consultorio.Domain.Entities;
public class Utente { public int UtenteId { get; set; } public string Codigo { get; set; } = ""; public string Nome { get; set; } = ""; public DateTime DataNascimento { get; set; } public string? Telefone { get; set; } public string? Email { get; set; } public string? Localidade { get; set; } public bool Ativo { get; set; } = true; }
