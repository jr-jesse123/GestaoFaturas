using FluentAssertions;
using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestaoFaturas.Tests.Data;

public class RepositoryTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public RepositoryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    private ApplicationDbContext CreateCleanContext()
    {
        var context = _fixture.CreateContext();
        context.Database.EnsureCreated();
        
        // Clean up existing data in correct order (respect foreign key constraints)
        context.InvoiceHistories.RemoveRange(context.InvoiceHistories);
        context.Invoices.RemoveRange(context.Invoices);
        context.ResponsiblePersons.RemoveRange(context.ResponsiblePersons);
        context.CostCenters.RemoveRange(context.CostCenters);
        context.Clients.RemoveRange(context.Clients);
        context.SaveChanges();
        
        return context;
    }

    #region GetById Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            Email = "test@company.com"
        };
        
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(client.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(client.Id);
        result.CompanyName.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { CompanyName = "Company 1", TaxId = "12345678901234", Email = "test1@company.com" },
            new Client { CompanyName = "Company 2", TaxId = "98765432109876", Email = "test2@company.com" },
            new Client { CompanyName = "Company 3", TaxId = "11111111111111", Email = "test3@company.com" }
        };
        
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.CompanyName == "Company 1");
        result.Should().Contain(c => c.CompanyName == "Company 2");
        result.Should().Contain(c => c.CompanyName == "Company 3");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEntities()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Find Tests

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredEntities()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { CompanyName = "Active Company 1", TaxId = "12345678901234", Email = "test1@company.com", IsActive = true },
            new Client { CompanyName = "Inactive Company", TaxId = "98765432109876", Email = "test2@company.com", IsActive = false },
            new Client { CompanyName = "Active Company 2", TaxId = "11111111111111", Email = "test3@company.com", IsActive = true }
        };
        
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(c => c.IsActive);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task FindAsync_WithIncludes_ShouldReturnEntitiesWithRelatedData()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            Email = "test@company.com"
        };
        
        var costCenter = new CostCenter
        {
            Client = client,
            Code = "CC001",
            Name = "Test Cost Center"
        };
        
        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(
            c => c.Id == client.Id,
            includeProperties: "CostCenters"
        );

        // Assert
        result.Should().HaveCount(1);
        var foundClient = result.First();
        foundClient.CostCenters.Should().HaveCount(1);
        foundClient.CostCenters.First().Code.Should().Be("CC001");
    }

    #endregion

    #region Add Tests

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            CompanyName = "New Company",
            TaxId = "12345678901234",
            Email = "new@company.com"
        };

        // Act
        await repository.AddAsync(client);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Clients.FirstOrDefaultAsync(c => c.TaxId == "12345678901234");
        saved.Should().NotBeNull();
        saved!.CompanyName.Should().Be("New Company");
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleEntities()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { CompanyName = "Company 1", TaxId = "12345678901234", Email = "test1@company.com" },
            new Client { CompanyName = "Company 2", TaxId = "98765432109876", Email = "test2@company.com" }
        };

        // Act
        await repository.AddRangeAsync(clients);
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Clients.CountAsync();
        count.Should().Be(2);
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_ShouldMarkEntityAsModified()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            Id = 1,
            CompanyName = "Updated Company",
            TaxId = "12345678901234",
            Email = "updated@company.com"
        };

        // Act
        repository.Update(client);

        // Assert
        var entry = context.Entry(client);
        entry.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public void UpdateRange_ShouldMarkMultipleEntitiesAsModified()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { Id = 1, CompanyName = "Company 1", TaxId = "12345678901234", Email = "test1@company.com" },
            new Client { Id = 2, CompanyName = "Company 2", TaxId = "98765432109876", Email = "test2@company.com" }
        };

        // Act
        repository.UpdateRange(clients);

        // Assert
        foreach (var client in clients)
        {
            context.Entry(client).State.Should().Be(EntityState.Modified);
        }
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Remove_ShouldMarkEntityAsDeleted()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            Id = 1,
            CompanyName = "To Delete",
            TaxId = "12345678901234",
            Email = "delete@company.com"
        };
        
        context.Clients.Attach(client);

        // Act
        repository.Remove(client);

        // Assert
        var entry = context.Entry(client);
        entry.State.Should().Be(EntityState.Deleted);
    }

    [Fact]
    public void RemoveRange_ShouldMarkMultipleEntitiesAsDeleted()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { Id = 1, CompanyName = "Company 1", TaxId = "12345678901234", Email = "test1@company.com" },
            new Client { Id = 2, CompanyName = "Company 2", TaxId = "98765432109876", Email = "test2@company.com" }
        };
        
        foreach (var client in clients)
        {
            context.Clients.Attach(client);
        }

        // Act
        repository.RemoveRange(clients);

        // Assert
        foreach (var client in clients)
        {
            context.Entry(client).State.Should().Be(EntityState.Deleted);
        }
    }

    #endregion

    #region Count Tests

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { CompanyName = "Company 1", TaxId = "12345678901234", Email = "test1@company.com" },
            new Client { CompanyName = "Company 2", TaxId = "98765432109876", Email = "test2@company.com" },
            new Client { CompanyName = "Company 3", TaxId = "11111111111111", Email = "test3@company.com" }
        };
        
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        // Act
        var count = await repository.CountAsync();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var clients = new[]
        {
            new Client { CompanyName = "Active 1", TaxId = "12345678901234", Email = "test1@company.com", IsActive = true },
            new Client { CompanyName = "Inactive", TaxId = "98765432109876", Email = "test2@company.com", IsActive = false },
            new Client { CompanyName = "Active 2", TaxId = "11111111111111", Email = "test3@company.com", IsActive = true }
        };
        
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        // Act
        var count = await repository.CountAsync(c => c.IsActive);

        // Assert
        count.Should().Be(2);
    }

    #endregion

    #region Exists Tests

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);
        
        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            Email = "test@company.com"
        };
        
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(c => c.TaxId == "12345678901234");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Arrange
        using var context = CreateCleanContext();
        var repository = new Repository<Client>(context);

        // Act
        var exists = await repository.ExistsAsync(c => c.TaxId == "99999999999999");

        // Assert
        exists.Should().BeFalse();
    }

    #endregion
}