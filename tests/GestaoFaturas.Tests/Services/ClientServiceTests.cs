using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.DTOs;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestaoFaturas.Tests.Services;

public class ClientServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IClientRepository> _mockClientRepository;
    private readonly Mock<ILogger<ClientService>> _mockLogger;
    private readonly ClientService _clientService;

    public ClientServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockClientRepository = new Mock<IClientRepository>();
        _mockLogger = new Mock<ILogger<ClientService>>();

        _mockUnitOfWork.Setup(x => x.Clients).Returns(_mockClientRepository.Object);

        _clientService = new ClientService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    #region CreateClientAsync Tests

    [Fact]
    public async Task CreateClientAsync_ShouldCreateClient_WhenValidDto()
    {
        // Arrange
        var createDto = new CreateClientDto
        {
            CompanyName = "Test Company",
            TradeName = "Test Trade",
            TaxId = "12345678901234",
            Email = "test@company.com",
            Phone = "11999999999",
            Address = "123 Test Street",
            ContactPerson = "John Doe"
        };

        _mockClientRepository
            .Setup(x => x.GetByTaxIdAsync(createDto.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _clientService.CreateClientAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.CompanyName, result.CompanyName);
        Assert.Equal(createDto.TradeName, result.TradeName);
        Assert.Equal(createDto.TaxId, result.TaxId);
        Assert.Equal(createDto.Email, result.Email);
        Assert.Equal(createDto.Phone, result.Phone);
        Assert.Equal(createDto.Address, result.Address);
        Assert.Equal(createDto.ContactPerson, result.ContactPerson);
        Assert.True(result.IsActive);

        _mockClientRepository.Verify(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClientAsync_ShouldThrowException_WhenTaxIdAlreadyExists()
    {
        // Arrange
        var createDto = new CreateClientDto
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234"
        };

        var existingClient = new Client { Id = 1, TaxId = "12345678901234", CompanyName = "Existing Company" };

        _mockClientRepository
            .Setup(x => x.GetByTaxIdAsync(createDto.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _clientService.CreateClientAsync(createDto));

        Assert.Contains("already exists", exception.Message);
        _mockClientRepository.Verify(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetClientByIdAsync Tests

    [Fact]
    public async Task GetClientByIdAsync_ShouldReturnClientDto_WhenClientExists()
    {
        // Arrange
        var clientId = 1;
        var client = new Client
        {
            Id = clientId,
            CompanyName = "Test Company",
            TradeName = "Test Trade",
            TaxId = "12345678901234",
            Email = "test@company.com",
            Phone = "11999999999",
            Address = "123 Test Street",
            ContactPerson = "John Doe",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act
        var result = await _clientService.GetClientByIdAsync(clientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.Id, result.Id);
        Assert.Equal(client.CompanyName, result.CompanyName);
        Assert.Equal(client.TradeName, result.TradeName);
        Assert.Equal(client.TaxId, result.TaxId);
        Assert.Equal(client.Email, result.Email);
    }

    [Fact]
    public async Task GetClientByIdAsync_ShouldReturnNull_WhenClientNotFound()
    {
        // Arrange
        var clientId = 999;

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act
        var result = await _clientService.GetClientByIdAsync(clientId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetClientsAsync Tests

    [Fact]
    public async Task GetClientsAsync_ShouldReturnPagedClients_WhenNoFilters()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, CompanyName = "Company A", TaxId = "111", IsActive = true },
            new Client { Id = 2, CompanyName = "Company B", TaxId = "222", IsActive = true },
            new Client { Id = 3, CompanyName = "Company C", TaxId = "333", IsActive = true }
        };

        _mockClientRepository
            .Setup(x => x.GetActiveClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        // Act
        var result = await _clientService.GetClientsAsync(pageNumber: 1, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(3, result.Items.Count());
    }

    [Fact]
    public async Task GetClientsAsync_ShouldReturnFilteredClients_WhenSearchProvided()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, CompanyName = "Test Company A", TaxId = "111", Email = "test@a.com", IsActive = true },
            new Client { Id = 2, CompanyName = "Company B", TaxId = "222", Email = "info@b.com", IsActive = true },
            new Client { Id = 3, CompanyName = "Test Company C", TaxId = "333", Email = "test@c.com", IsActive = true }
        };

        _mockClientRepository
            .Setup(x => x.GetActiveClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        // Act
        var result = await _clientService.GetClientsAsync(pageNumber: 1, pageSize: 10, search: "Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, item => Assert.Contains("Test", item.CompanyName));
    }

    #endregion

    #region UpdateClientAsync Tests

    [Fact]
    public async Task UpdateClientAsync_ShouldUpdateClient_WhenValidDto()
    {
        // Arrange
        var clientId = 1;
        var existingClient = new Client
        {
            Id = clientId,
            CompanyName = "Old Company",
            TradeName = "Old Trade",
            TaxId = "11111111111111",
            Email = "old@company.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateClientDto
        {
            CompanyName = "Updated Company",
            TradeName = "Updated Trade",
            TaxId = "22222222222222",
            Email = "updated@company.com",
            Phone = "11777777777",
            Address = "Updated Address",
            ContactPerson = "Updated Contact"
        };

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClient);

        _mockClientRepository
            .Setup(x => x.GetByTaxIdAsync(updateDto.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _clientService.UpdateClientAsync(clientId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.CompanyName, result.CompanyName);
        Assert.Equal(updateDto.TradeName, result.TradeName);
        Assert.Equal(updateDto.TaxId, result.TaxId);
        Assert.Equal(updateDto.Email, result.Email);
        Assert.Equal(updateDto.Phone, result.Phone);
        Assert.Equal(updateDto.Address, result.Address);
        Assert.Equal(updateDto.ContactPerson, result.ContactPerson);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateClientAsync_ShouldReturnNull_WhenClientNotFound()
    {
        // Arrange
        var clientId = 999;
        var updateDto = new UpdateClientDto { CompanyName = "Updated Company", TaxId = "12345678901234" };

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act
        var result = await _clientService.UpdateClientAsync(clientId, updateDto);

        // Assert
        Assert.Null(result);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateClientAsync_ShouldThrowException_WhenTaxIdExistsForDifferentClient()
    {
        // Arrange
        var clientId = 1;
        var existingClient = new Client
        {
            Id = clientId,
            CompanyName = "Client 1",
            TaxId = "11111111111111",
            IsActive = true
        };

        var anotherClient = new Client
        {
            Id = 2,
            CompanyName = "Client 2",
            TaxId = "22222222222222",
            IsActive = true
        };

        var updateDto = new UpdateClientDto
        {
            CompanyName = "Updated Company",
            TaxId = "22222222222222" // Same as another client
        };

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClient);

        _mockClientRepository
            .Setup(x => x.GetByTaxIdAsync(updateDto.TaxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _clientService.UpdateClientAsync(clientId, updateDto));

        Assert.Contains("already exists", exception.Message);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeactivateClientAsync Tests

    [Fact]
    public async Task DeactivateClientAsync_ShouldDeactivateClient_WhenClientExists()
    {
        // Arrange
        var clientId = 1;
        var client = new Client
        {
            Id = clientId,
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            IsActive = true
        };

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _clientService.DeactivateClientAsync(clientId);

        // Assert
        Assert.True(result);
        Assert.False(client.IsActive);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateClientAsync_ShouldReturnFalse_WhenClientNotFound()
    {
        // Arrange
        var clientId = 999;

        _mockClientRepository
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act
        var result = await _clientService.DeactivateClientAsync(clientId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Search and Filter Tests

    [Fact]
    public async Task GetClientsAsync_ShouldFilterByTaxId_WhenSearchMatchesTaxId()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, CompanyName = "Company A", TaxId = "12345678901234", IsActive = true },
            new Client { Id = 2, CompanyName = "Company B", TaxId = "98765432109876", IsActive = true },
        };

        _mockClientRepository
            .Setup(x => x.GetActiveClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        // Act
        var result = await _clientService.GetClientsAsync(pageNumber: 1, pageSize: 10, search: "12345");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalItems);
        Assert.Contains("12345", result.Items.First().TaxId);
    }

    [Fact]
    public async Task GetClientsAsync_ShouldFilterByEmail_WhenSearchMatchesEmail()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client { Id = 1, CompanyName = "Company A", TaxId = "111", Email = "contact@companya.com", IsActive = true },
            new Client { Id = 2, CompanyName = "Company B", TaxId = "222", Email = "info@companyb.com", IsActive = true },
        };

        _mockClientRepository
            .Setup(x => x.GetActiveClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        // Act
        var result = await _clientService.GetClientsAsync(pageNumber: 1, pageSize: 10, search: "contact@");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalItems);
        Assert.Contains("contact@", result.Items.First().Email);
    }

    [Fact]
    public async Task GetClientsAsync_ShouldHandlePagination_Correctly()
    {
        // Arrange
        var clients = new List<Client>();
        for (int i = 1; i <= 25; i++)
        {
            clients.Add(new Client { Id = i, CompanyName = $"Company {i}", TaxId = $"{i:D14}", IsActive = true });
        }

        _mockClientRepository
            .Setup(x => x.GetActiveClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clients);

        // Act - Get page 2 with 10 items per page
        var result = await _clientService.GetClientsAsync(pageNumber: 2, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages); // 25 items / 10 per page = 3 pages
        Assert.Equal(10, result.Items.Count());
    }

    #endregion
}