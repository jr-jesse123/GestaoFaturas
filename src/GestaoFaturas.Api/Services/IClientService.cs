using GestaoFaturas.Api.DTOs;

namespace GestaoFaturas.Api.Services;

public interface IClientService
{
    Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, CancellationToken cancellationToken = default);
    Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ClientDto>> GetClientsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto updateClientDto, CancellationToken cancellationToken = default);
    Task<bool> DeactivateClientAsync(int id, CancellationToken cancellationToken = default);
}