using System.ComponentModel.DataAnnotations;
using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.DTOs;

public class UpdateClientDto
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

    public void UpdateEntity(Client client)
    {
        client.CompanyName = CompanyName;
        client.TradeName = TradeName;
        client.TaxId = TaxId;
        client.Email = Email;
        client.Phone = Phone;
        client.Address = Address;
        client.ContactPerson = ContactPerson;
    }
}