using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoFaturas.Api.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    public int CostCenterId { get; set; }

    [Required]
    public int InvoiceStatusId { get; set; }

    [Required]
    [StringLength(100)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public DateTime ServicePeriodStart { get; set; }

    [Required]
    public DateTime ServicePeriodEnd { get; set; }

    [StringLength(50)]
    public string? ServiceType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? DocumentPath { get; set; } // Path to uploaded invoice document

    public DateTime? ReceivedDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? PaidDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Client Client { get; set; } = null!;
    public virtual CostCenter CostCenter { get; set; } = null!;
    public virtual InvoiceStatus InvoiceStatus { get; set; } = null!;
    public virtual ICollection<InvoiceHistory> InvoiceHistories { get; set; } = new List<InvoiceHistory>();
}