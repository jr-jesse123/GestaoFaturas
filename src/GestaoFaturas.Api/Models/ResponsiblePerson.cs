using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class ResponsiblePerson
{
    public int Id { get; set; }

    [Required]
    public int CostCenterId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Position { get; set; }

    [StringLength(50)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual CostCenter CostCenter { get; set; } = null!;
}