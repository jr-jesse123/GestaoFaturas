using System.ComponentModel.DataAnnotations;
using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.DTOs;

public class CreateClientDto
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? TradeName { get; set; }

    [Required]
    [StringLength(14)]
    public string TaxId { get; set; } = string.Empty;

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

    public Client ToEntity()
    {
        return new Client
        {
            CompanyName = CompanyName,
            TradeName = TradeName,
            TaxId = TaxId,
            Email = Email,
            Phone = Phone,
            Address = Address,
            ContactPerson = ContactPerson,
            IsActive = true
        };
    }
}