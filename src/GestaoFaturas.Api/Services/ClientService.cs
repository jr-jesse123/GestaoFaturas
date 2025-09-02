using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.DTOs;
using GestaoFaturas.Api.Models;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Api.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IUnitOfWork unitOfWork, ILogger<ClientService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new client with TaxId: {TaxId}", createClientDto.TaxId);

        // Check if client with same TaxId already exists
        var existingClient = await _unitOfWork.Clients.GetByTaxIdAsync(createClientDto.TaxId, cancellationToken);
        if (existingClient != null)
        {
            _logger.LogWarning("Client with TaxId {TaxId} already exists", createClientDto.TaxId);
            throw new InvalidOperationException($"A client with Tax ID {createClientDto.TaxId} already exists.");
        }

        // Create new client entity
        var client = createClientDto.ToEntity();
        client.CreatedAt = DateTime.UtcNow;
        client.UpdatedAt = DateTime.UtcNow;

        // Add to repository
        await _unitOfWork.Clients.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created client with Id: {Id}", client.Id);

        return ClientDto.FromEntity(client);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving client with Id: {Id}", id);

        var client = await _unitOfWork.Clients.GetByIdAsync(id, cancellationToken);
        
        if (client == null)
        {
            _logger.LogDebug("Client with Id: {Id} not found", id);
            return null;
        }

        return ClientDto.FromEntity(client);
    }

    public async Task<PagedResult<ClientDto>> GetClientsAsync(int pageNumber = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving clients - Page: {PageNumber}, Size: {PageSize}, Search: {Search}", pageNumber, pageSize, search);

        var clients = await _unitOfWork.Clients.GetActiveClientsAsync(cancellationToken);
        
        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLowerInvariant();
            clients = clients.Where(c => 
                c.CompanyName.ToLowerInvariant().Contains(searchTerm) ||
                (c.TradeName != null && c.TradeName.ToLowerInvariant().Contains(searchTerm)) ||
                c.TaxId.Contains(searchTerm) ||
                (c.Email != null && c.Email.ToLowerInvariant().Contains(searchTerm))
            );
        }

        // Convert to DTOs
        var clientDtos = clients.Select(ClientDto.FromEntity);

        // Create paged result
        var pagedResult = PagedResult<ClientDto>.Create(clientDtos, pageNumber, pageSize);

        _logger.LogDebug("Retrieved {TotalItems} clients, returning page {PageNumber} of {TotalPages}", 
            pagedResult.TotalItems, pagedResult.PageNumber, pagedResult.TotalPages);

        return pagedResult;
    }

    public async Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto updateClientDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating client with Id: {Id}", id);

        var client = await _unitOfWork.Clients.GetByIdAsync(id, cancellationToken);
        if (client == null)
        {
            _logger.LogWarning("Client with Id: {Id} not found for update", id);
            return null;
        }

        // Check if TaxId is being changed and if new TaxId already exists
        if (client.TaxId != updateClientDto.TaxId)
        {
            var existingClientWithTaxId = await _unitOfWork.Clients.GetByTaxIdAsync(updateClientDto.TaxId, cancellationToken);
            if (existingClientWithTaxId != null && existingClientWithTaxId.Id != id)
            {
                _logger.LogWarning("Client with TaxId {TaxId} already exists", updateClientDto.TaxId);
                throw new InvalidOperationException($"A client with Tax ID {updateClientDto.TaxId} already exists.");
            }
        }

        // Update entity
        updateClientDto.UpdateEntity(client);
        client.UpdatedAt = DateTime.UtcNow;

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated client with Id: {Id}", id);

        return ClientDto.FromEntity(client);
    }

    public async Task<bool> DeactivateClientAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating client with Id: {Id}", id);

        var client = await _unitOfWork.Clients.GetByIdAsync(id, cancellationToken);
        if (client == null)
        {
            _logger.LogWarning("Client with Id: {Id} not found for deactivation", id);
            return false;
        }

        client.IsActive = false;
        client.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deactivated client with Id: {Id}", id);

        return true;
    }
}