using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Data.Repositories;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestaoFaturas.Tests.Repositories;

public class ResponsiblePersonRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ResponsiblePersonRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private ApplicationDbContext CreateContext()
    {
        var context = _fixture.CreateContext();
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddAsync_Should_Add_ResponsiblePerson_To_Database()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);
        var unitOfWork = new UnitOfWork(context);

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+55 11 98765-4321",
            Role = "Finance Manager",
            Department = "Finance",
            IsPrimaryContact = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(responsiblePerson);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var savedPerson = await context.ResponsiblePersons.FirstOrDefaultAsync(rp => rp.Email == "john.doe@example.com");
        Assert.NotNull(savedPerson);
        Assert.Equal("John Doe", savedPerson.Name);
        Assert.Equal(client.Id, savedPerson.ClientId);
        Assert.True(savedPerson.IsPrimaryContact);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_ResponsiblePerson_With_Client()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var responsiblePerson = new ResponsiblePerson
        {
            Name = "Jane Smith",
            Email = "jane.smith@example.com",
            Client = client,
            ClientId = client.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.ResponsiblePersons.AddAsync(responsiblePerson);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithClientAsync(responsiblePerson.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane Smith", result.Name);
        Assert.NotNull(result.Client);
        Assert.Equal("Test Company", result.Client.CompanyName);
    }

    [Fact]
    public async Task GetByClientIdAsync_Should_Return_All_ResponsiblePersons_For_Client()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Multi Contact Company",
            TaxId = "98765432109876",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var responsiblePersons = new List<ResponsiblePerson>
        {
            new ResponsiblePerson
            {
                ClientId = client.Id,
                Name = "Contact 1",
                Email = "contact1@example.com",
                IsPrimaryContact = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ResponsiblePerson
            {
                ClientId = client.Id,
                Name = "Contact 2",
                Email = "contact2@example.com",
                IsPrimaryContact = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ResponsiblePerson
            {
                ClientId = client.Id,
                Name = "Contact 3",
                Email = "contact3@example.com",
                IsPrimaryContact = false,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.ResponsiblePersons.AddRangeAsync(responsiblePersons);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByClientIdAsync(client.Id);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(3, resultList.Count);
        Assert.Contains(resultList, rp => rp.Name == "Contact 1");
        Assert.Contains(resultList, rp => rp.Name == "Contact 2");
        Assert.Contains(resultList, rp => rp.Name == "Contact 3");
    }

    [Fact]
    public async Task GetPrimaryContactAsync_Should_Return_Only_Primary_Contact()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Primary Contact Test",
            TaxId = "11111111111111",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var primaryContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Primary Contact",
            Email = "primary@example.com",
            IsPrimaryContact = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var secondaryContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Secondary Contact",
            Email = "secondary@example.com",
            IsPrimaryContact = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddRangeAsync(primaryContact, secondaryContact);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetPrimaryContactAsync(client.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Primary Contact", result.Name);
        Assert.True(result.IsPrimaryContact);
    }

    [Fact]
    public async Task HasPrimaryContactAsync_Should_Return_True_When_Primary_Exists()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Has Primary Test",
            TaxId = "22222222222222",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var primaryContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Primary",
            Email = "primary@test.com",
            IsPrimaryContact = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddAsync(primaryContact);
        await context.SaveChangesAsync();

        // Act
        var hasPrimary = await repository.HasPrimaryContactAsync(client.Id);
        var hasPrimaryExcluding = await repository.HasPrimaryContactAsync(client.Id, excludeId: primaryContact.Id);

        // Assert
        Assert.True(hasPrimary);
        Assert.False(hasPrimaryExcluding);
    }

    [Fact]
    public async Task Update_Should_Modify_ResponsiblePerson_Data()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);
        var unitOfWork = new UnitOfWork(context);

        var client = new Client
        {
            CompanyName = "Update Test Company",
            TaxId = "33333333333333",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Original Name",
            Email = "original@example.com",
            IsPrimaryContact = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddAsync(responsiblePerson);
        await context.SaveChangesAsync();

        // Act
        responsiblePerson.Name = "Updated Name";
        responsiblePerson.Email = "updated@example.com";
        responsiblePerson.IsPrimaryContact = true;
        responsiblePerson.UpdatedAt = DateTime.UtcNow;

        repository.Update(responsiblePerson);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var updated = await context.ResponsiblePersons.FindAsync(responsiblePerson.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal("updated@example.com", updated.Email);
        Assert.True(updated.IsPrimaryContact);
    }

    [Fact]
    public async Task Remove_Should_Delete_ResponsiblePerson_From_Database()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);
        var unitOfWork = new UnitOfWork(context);

        var client = new Client
        {
            CompanyName = "Delete Test Company",
            TaxId = "44444444444444",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "To Be Deleted",
            Email = "delete@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddAsync(responsiblePerson);
        await context.SaveChangesAsync();

        // Act
        repository.Remove(responsiblePerson);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var deleted = await context.ResponsiblePersons.FindAsync(responsiblePerson.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetActiveByClientIdAsync_Should_Return_Only_Active_Contacts()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Active Filter Test",
            TaxId = "55555555555555",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var activeContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Active Contact",
            Email = "active@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var inactiveContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Inactive Contact",
            Email = "inactive@example.com",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddRangeAsync(activeContact, inactiveContact);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetActiveByClientIdAsync(client.Id);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal("Active Contact", resultList[0].Name);
        Assert.True(resultList[0].IsActive);
    }

    [Fact]
    public async Task EmailExistsAsync_Should_Check_Email_Uniqueness()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ResponsiblePersonRepository(context);

        var client = new Client
        {
            CompanyName = "Email Check Test",
            TaxId = "66666666666666",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        var existingContact = new ResponsiblePerson
        {
            ClientId = client.Id,
            Name = "Existing Contact",
            Email = "existing@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ResponsiblePersons.AddAsync(existingContact);
        await context.SaveChangesAsync();

        // Act
        var emailExists = await repository.EmailExistsAsync("existing@example.com", client.Id);
        var emailExistsExcluding = await repository.EmailExistsAsync("existing@example.com", client.Id, excludeId: existingContact.Id);
        var emailDoesNotExist = await repository.EmailExistsAsync("new@example.com", client.Id);

        // Assert
        Assert.True(emailExists);
        Assert.False(emailExistsExcluding);
        Assert.False(emailDoesNotExist);
    }
}