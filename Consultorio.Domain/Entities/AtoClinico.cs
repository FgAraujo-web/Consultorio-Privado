namespace Consultorio.Domain.Entities;
public class AtoClinico { public int AtoId { get; set; } public int ConsultaId { get; set; } public Consulta? Consulta { get; set; } public int TipoAtoId { get; set; } public TipoAto? TipoAto { get; set; } public DateTime DataAto { get; set; } public string? Descricao { get; set; } public decimal Valor { get; set; } public bool Anulado { get; set; } }
