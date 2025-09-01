using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class CostCenter
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    // Hierarchical structure support
    public int? ParentCostCenterId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Client Client { get; set; } = null!;
    public virtual CostCenter? ParentCostCenter { get; set; }
    public virtual ICollection<CostCenter> ChildCostCenters { get; set; } = new List<CostCenter>();
    public virtual ICollection<ResponsiblePerson> ResponsiblePersons { get; set; } = new List<ResponsiblePerson>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}