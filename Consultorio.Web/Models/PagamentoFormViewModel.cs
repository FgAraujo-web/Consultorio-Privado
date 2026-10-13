using System.ComponentModel.DataAnnotations;
namespace Consultorio.Web.Models;
public class PagamentoFormViewModel
{
    public int PagamentoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma consulta.")]
    [Display(Name = "Consulta")]
    public int ConsultaId { get; set; }
    [Range(typeof(decimal), "0.01", "99999999", ErrorMessage = "Indique um valor superior a zero.")]
    [Display(Name = "Valor (€)")]
    public decimal Valor { get; set; }
    [Required(ErrorMessage = "Indique a data do pagamento.")]
    [Display(Name = "Data do pagamento")]
    public DateTime DataPagamento { get; set; } = DateTime.Now;
    [Required(ErrorMessage = "Selecione o método de pagamento.")]
    [StringLength(40)]
    [Display(Name = "Método")]
    public string Metodo { get; set; } = "Cartao";
    [Required(ErrorMessage = "Indique a referência fictícia.")]
    [StringLength(100)]
    [Display(Name = "Referência fictícia")]
    public string ReferenciaFicticia { get; set; } = "";
}
