using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? TradeName { get; set; }

    [Required]
    [StringLength(14)]
    public string TaxId { get; set; } = string.Empty; // CNPJ or CPF

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(20)]
    [Phone]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<ResponsiblePerson> ResponsiblePersons { get; set; } = new List<ResponsiblePerson>();
}