namespace Consultorio.Domain.Entities;
public class Pagamento { public int PagamentoId { get; set; } public int ConsultaId { get; set; } public Consulta? Consulta { get; set; } public decimal Valor { get; set; } public DateTime DataPagamento { get; set; } public string Metodo { get; set; } = "Cartao"; public string ReferenciaFicticia { get; set; } = ""; }
