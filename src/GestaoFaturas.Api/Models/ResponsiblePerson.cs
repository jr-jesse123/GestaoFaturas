using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class ResponsiblePerson
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Role { get; set; }

    [StringLength(50)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
    public bool ReceivesNotifications { get; set; } = true;
    public bool IsPrimaryContact { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Client Client { get; set; } = null!;
    public virtual ICollection<CostCenterResponsible> CostCenterResponsibles { get; set; } = new List<CostCenterResponsible>();
}