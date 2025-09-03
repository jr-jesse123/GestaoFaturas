using System.ComponentModel.DataAnnotations;

namespace GestaoFaturas.Api.Models;

public class CostCenterResponsible
{
    [Required]
    public int CostCenterId { get; set; }

    [Required]
    public int ResponsiblePersonId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsPrimary { get; set; } = false;

    // Navigation properties
    public virtual CostCenter CostCenter { get; set; } = null!;
    public virtual ResponsiblePerson ResponsiblePerson { get; set; } = null!;
}