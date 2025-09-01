using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Tests.Entities;

public class ClientEntityTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public ClientEntityTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Client_ShouldCreateValidEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Test Telecom Company",
            TradeName = "Test Telecom",
            TaxId = "12345678000123",
            Email = "contact@testtelecom.com",
            Phone = "+5511999999999",
            Address = "123 Test Street, Test City, Test State",
            ContactPerson = "John Doe"
        };

        // Act
        context.Clients.Add(client);
        var result = await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.True(client.Id > 0);
        Assert.True(client.CreatedAt > DateTime.MinValue);
        Assert.True(client.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task Client_ShouldReadEntityById()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Read Test Company",
            TradeName = "Read Test",
            TaxId = "98765432000198",
            Email = "read@test.com",
            Phone = "+5511888888888",
            Address = "456 Read Avenue",
            ContactPerson = "Jane Smith"
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();
        var clientId = client.Id;

        // Act
        var retrievedClient = await context.Clients.FindAsync(clientId);

        // Assert
        Assert.NotNull(retrievedClient);
        Assert.Equal("Read Test Company", retrievedClient.CompanyName);
        Assert.Equal("98765432000198", retrievedClient.TaxId);
        Assert.Equal("read@test.com", retrievedClient.Email);
    }

    [Fact]
    public async Task Client_ShouldUpdateEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Update Test Company",
            TradeName = "Update Test",
            TaxId = "11111111000111",
            Email = "update@test.com",
            Phone = "+5511777777777",
            Address = "789 Update Blvd",
            ContactPerson = "Bob Johnson"
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();
        var originalUpdatedAt = client.UpdatedAt;

        // Act
        client.CompanyName = "Updated Company Name";
        client.Email = "updated@test.com";
        await context.SaveChangesAsync();

        // Assert
        var updatedClient = await context.Clients.FindAsync(client.Id);
        Assert.NotNull(updatedClient);
        Assert.Equal("Updated Company Name", updatedClient.CompanyName);
        Assert.Equal("updated@test.com", updatedClient.Email);
        Assert.True(updatedClient.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public async Task Client_ShouldDeleteEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Delete Test Company",
            TradeName = "Delete Test",
            TaxId = "22222222000222",
            Email = "delete@test.com",
            Phone = "+5511666666666",
            Address = "321 Delete Drive",
            ContactPerson = "Alice Brown"
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();
        var clientId = client.Id;

        // Act
        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        // Assert
        var deletedClient = await context.Clients.FindAsync(clientId);
        Assert.Null(deletedClient);
    }

    [Fact]
    public async Task Client_ShouldEnforceUniqueConstraintOnTaxId()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client1 = new Client
        {
            CompanyName = "First Company",
            TradeName = "First",
            TaxId = "33333333000333",
            Email = "first@test.com",
            Phone = "+5511555555555",
            Address = "111 First Street",
            ContactPerson = "First Person"
        };

        var client2 = new Client
        {
            CompanyName = "Second Company",
            TradeName = "Second",
            TaxId = "33333333000333", // Same TaxId
            Email = "second@test.com",
            Phone = "+5511444444444",
            Address = "222 Second Street",
            ContactPerson = "Second Person"
        };

        // Act & Assert
        context.Clients.Add(client1);
        await context.SaveChangesAsync(); // First should succeed

        context.Clients.Add(client2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task Client_ShouldValidateRequiredFields()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Test that null/empty required fields are handled properly
        var clientWithNullCompanyName = new Client
        {
            CompanyName = null!,
            TaxId = "44444444000444",
            Email = "test@withoutname.com"
        };

        var clientWithNullTaxId = new Client
        {
            CompanyName = "Test Company",
            TaxId = null!,
            Email = "test@withouttaxid.com"
        };

        // Act & Assert - Database should enforce NOT NULL constraints
        context.Clients.Add(clientWithNullCompanyName);
        var ex1 = await Record.ExceptionAsync(async () => await context.SaveChangesAsync());
        Assert.NotNull(ex1);

        // Clear tracking and test second scenario
        context.ChangeTracker.Clear();
        context.Clients.Add(clientWithNullTaxId);
        var ex2 = await Record.ExceptionAsync(async () => await context.SaveChangesAsync());
        Assert.NotNull(ex2);
    }

    [Fact]
    public async Task Client_ShouldSetAuditFieldsAutomatically()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Audit Test Company",
            TradeName = "Audit Test",
            TaxId = "55555555000555",
            Email = "audit@test.com",
            Phone = "+5511333333333",
            Address = "555 Audit Avenue",
            ContactPerson = "Audit Person"
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(client.CreatedAt >= beforeCreate);
        Assert.True(client.UpdatedAt >= beforeCreate);
        Assert.True(client.CreatedAt <= DateTime.UtcNow);
        Assert.True(client.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Client_ShouldQueryByTaxId()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            CompanyName = "Query Test Company",
            TradeName = "Query Test",
            TaxId = "66666666000666",
            Email = "query@test.com",
            Phone = "+5511222222222",
            Address = "666 Query Road",
            ContactPerson = "Query Person"
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Act
        var queriedClient = await context.Clients
            .Where(c => c.TaxId == "66666666000666")
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(queriedClient);
        Assert.Equal("Query Test Company", queriedClient.CompanyName);
        Assert.Equal("66666666000666", queriedClient.TaxId);
    }
}