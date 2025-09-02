using System.ComponentModel.DataAnnotations;
using GestaoFaturas.Api.DTOs;

namespace GestaoFaturas.Api.Models;

public class ClientInputModel
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Trade name cannot exceed 200 characters")]
    [Display(Name = "Trade Name")]
    public string? TradeName { get; set; }

    [Required(ErrorMessage = "Tax ID is required")]
    [StringLength(14, ErrorMessage = "Tax ID must be exactly 14 characters")]
    [MinLength(14, ErrorMessage = "Tax ID must be exactly 14 characters")]
    [Display(Name = "Tax ID (CNPJ/CPF)")]
    public string TaxId { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
    [Display(Name = "Contact Person")]
    public string? ContactPerson { get; set; }

    public CreateClientDto ToCreateDto()
    {
        return new CreateClientDto
        {
            CompanyName = CompanyName,
            TradeName = TradeName,
            TaxId = TaxId,
            Email = Email,
            Phone = Phone,
            Address = Address,
            ContactPerson = ContactPerson
        };
    }

    public UpdateClientDto ToUpdateDto()
    {
        return new UpdateClientDto
        {
            CompanyName = CompanyName,
            TradeName = TradeName,
            TaxId = TaxId,
            Email = Email,
            Phone = Phone,
            Address = Address,
            ContactPerson = ContactPerson
        };
    }

    public static ClientInputModel FromDto(ClientDto client)
    {
        return new ClientInputModel
        {
            CompanyName = client.CompanyName,
            TradeName = client.TradeName,
            TaxId = client.TaxId,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            ContactPerson = client.ContactPerson
        };
    }
}