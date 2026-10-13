using System.ComponentModel.DataAnnotations;

namespace Consultorio.Web.Models;

public class ConsultaFormViewModel
{
    public int ConsultaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um utente.")]
    [Display(Name = "Utente")]
    public int UtenteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um médico.")]
    [Display(Name = "Médico")]
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "Indique a data e hora da consulta.")]
    [Display(Name = "Data e hora")]
    public DateTime? DataHora { get; set; }

    [Range(15, 60, ErrorMessage = "Selecione uma duração válida (15, 30, 45 ou 60 minutos).")]
    [Display(Name = "Duração (minutos)")]
    public int DuracaoMinutos { get; set; } = 30;

    [StringLength(1000, ErrorMessage = "As observações não podem exceder 1000 caracteres.")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }
}
