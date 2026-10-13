namespace Consultorio.Domain.Entities;
public class TipoAto { public int TipoAtoId { get; set; } public string Codigo { get; set; } = ""; public string Designacao { get; set; } = ""; public decimal PrecoBase { get; set; } public bool Ativo { get; set; } = true; }
