using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class InvoiceStatus
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string? Color { get; set; } // For UI display

    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFinal { get; set; } = false; // Indicates if this is a final status

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}