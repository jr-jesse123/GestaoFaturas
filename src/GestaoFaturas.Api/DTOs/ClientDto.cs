using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.DTOs;

public record ClientDto
{
    public int Id { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string? TradeName { get; init; }
    public string TaxId { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? ContactPerson { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public static ClientDto FromEntity(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            CompanyName = client.CompanyName,
            TradeName = client.TradeName,
            TaxId = client.TaxId,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            ContactPerson = client.ContactPerson,
            IsActive = client.IsActive,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}