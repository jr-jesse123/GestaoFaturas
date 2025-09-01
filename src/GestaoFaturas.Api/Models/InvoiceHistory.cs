using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class InvoiceHistory
{
    public int Id { get; set; }

    [Required]
    public int InvoiceId { get; set; }

    [Required]
    public int FromStatusId { get; set; }

    [Required]
    public int ToStatusId { get; set; }

    [StringLength(500)]
    public string? ChangeReason { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    [StringLength(100)]
    public string? ChangedByUserId { get; set; } // References AspNetUser.Id

    [Required]
    public DateTime ChangedAt { get; set; }

    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual InvoiceStatus FromStatus { get; set; } = null!;
    public virtual InvoiceStatus ToStatus { get; set; } = null!;
}