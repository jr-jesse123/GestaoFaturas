using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Models;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Tests.Data;

public class DatabaseConstraintsTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public DatabaseConstraintsTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    private ApplicationDbContext CreateCleanContext()
    {
        var context = _fixture.CreateContext();
        context.Database.EnsureCreated();
        
        // Clean up existing data to ensure test isolation
        context.InvoiceHistories.RemoveRange(context.InvoiceHistories);
        context.Invoices.RemoveRange(context.Invoices);
        context.ResponsiblePersons.RemoveRange(context.ResponsiblePersons);
        context.CostCenters.RemoveRange(context.CostCenters);
        context.Clients.RemoveRange(context.Clients);
        context.InvoiceStatuses.RemoveRange(context.InvoiceStatuses);
        context.SaveChanges();
        
        return context;
    }

    private static string GetUniqueTaxId() => (DateTime.UtcNow.Ticks % 100000000000000).ToString("D14"); // Max 14 chars
    private static string GetUniqueEmail() => $"test{DateTime.UtcNow.Ticks % 1000000:D6}@example.com"; // Max 100 chars from Client model
    private static string GetUniqueCode() => $"CC{DateTime.UtcNow.Ticks % 100000:D5}"; // Max 50 chars from CostCenter model
    private static string GetUniqueInvoiceNumber() => $"INV{DateTime.UtcNow.Ticks % 100000:D5}"; // Max 100 chars from Invoice model

    #region Unique Constraints Tests

    [Fact]
    public async Task Client_ShouldEnforceUniqueTaxId()
    {
        // Arrange
        using var context = CreateCleanContext();
        var duplicateTaxId = GetUniqueTaxId();

        var client1 = new Client
        {
            CompanyName = "Company 1",
            TaxId = duplicateTaxId,
            Email = GetUniqueEmail()
        };

        var client2 = new Client
        {
            CompanyName = "Company 2", 
            TaxId = duplicateTaxId, // Duplicate TaxId
            Email = GetUniqueEmail()
        };

        // Act & Assert
        context.Clients.Add(client1);
        await context.SaveChangesAsync();

        context.Clients.Add(client2);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("duplicate key", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ix_clients_tax_id", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task CostCenter_ShouldEnforceUniqueCodePerClient()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var duplicateCode = GetUniqueCode();
        var costCenter1 = new CostCenter
        {
            ClientId = client.Id,
            Code = duplicateCode,
            Name = "Cost Center 1"
        };

        var costCenter2 = new CostCenter
        {
            ClientId = client.Id,
            Code = duplicateCode, // Duplicate Code for same client
            Name = "Cost Center 2"
        };

        // Act & Assert
        context.CostCenters.Add(costCenter1);
        await context.SaveChangesAsync();

        context.CostCenters.Add(costCenter2);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("duplicate key", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ix_cost_centers_client_code", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task CostCenter_ShouldAllowSameCodeForDifferentClients()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client1 = new Client
        {
            CompanyName = "Company 1",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var client2 = new Client
        {
            CompanyName = "Company 2",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        context.Clients.AddRange(client1, client2);
        await context.SaveChangesAsync();

        var costCenter1 = new CostCenter
        {
            ClientId = client1.Id,
            Code = GetUniqueCode(),
            Name = "Cost Center 1"
        };

        var costCenter2 = new CostCenter
        {
            ClientId = client2.Id,
            Code = GetUniqueCode(), // Same code but different client
            Name = "Cost Center 2"
        };

        // Act & Assert
        context.CostCenters.AddRange(costCenter1, costCenter2);
        await context.SaveChangesAsync(); // Should not throw
        
        Assert.Equal(2, await context.CostCenters.CountAsync());
    }

    [Fact]
    public async Task Invoice_ShouldEnforceUniqueInvoiceNumberPerClient()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var duplicateInvoiceNumber = GetUniqueInvoiceNumber();
        var invoice1 = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = duplicateInvoiceNumber,
            Amount = 100.00m,
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        var invoice2 = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = duplicateInvoiceNumber, // Duplicate invoice number for same client
            Amount = 200.00m,
            TotalAmount = 200.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.Add(invoice1);
        await context.SaveChangesAsync();

        context.Invoices.Add(invoice2);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("duplicate key", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ix_invoices_client_number", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldAllowSameInvoiceNumberForDifferentClients()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client1 = new Client
        {
            CompanyName = "Company 1",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var client2 = new Client
        {
            CompanyName = "Company 2",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter1 = new CostCenter
        {
            Client = client1,
            Code = GetUniqueCode(),
            Name = "Cost Center 1"
        };

        var costCenter2 = new CostCenter
        {
            Client = client2,
            Code = GetUniqueCode(),
            Name = "Cost Center 2"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.AddRange(client1, client2);
        context.CostCenters.AddRange(costCenter1, costCenter2);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice1 = new Invoice
        {
            ClientId = client1.Id,
            CostCenterId = costCenter1.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m,
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        var invoice2 = new Invoice
        {
            ClientId = client2.Id, // Different client
            CostCenterId = costCenter2.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(), // Same invoice number but different client
            Amount = 200.00m,
            TotalAmount = 200.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.AddRange(invoice1, invoice2);
        await context.SaveChangesAsync(); // Should not throw
        
        Assert.Equal(2, await context.Invoices.CountAsync());
    }

    #endregion

    #region Check Constraints Tests

    [Fact]
    public async Task Invoice_ShouldRejectNegativeAmount()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = -100.00m, // Negative amount
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("check constraint", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ck_invoices_amount_positive", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldRejectZeroAmount()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 0.00m, // Zero amount
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("check constraint", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ck_invoices_amount_positive", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldRejectNegativeTotalAmount()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m,
            TotalAmount = -100.00m, // Negative total amount
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("check constraint", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ck_invoices_totalamount_positive", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldRejectInvalidServicePeriod()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m,
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow, // Start after end
            ServicePeriodEnd = DateTime.UtcNow.AddDays(-30) 
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("check constraint", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ck_invoices_serviceperiod_valid", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldRejectInvalidDueDate()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m,
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(-30), // Due date before issue date
            ServicePeriodStart = DateTime.UtcNow.AddDays(-60),
            ServicePeriodEnd = DateTime.UtcNow.AddDays(-30)
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        Assert.Contains("check constraint", exception.InnerException?.Message?.ToLower());
        Assert.Contains("ck_invoices_duedate_valid", exception.InnerException?.Message?.ToLower());
    }

    [Fact]
    public async Task Invoice_ShouldAcceptValidData()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        await context.SaveChangesAsync();

        var invoice = new Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m, // Positive amount
            TotalAmount = 120.00m, // Positive total amount
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30), // Due date after issue date
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30), // Start before end
            ServicePeriodEnd = DateTime.UtcNow
        };

        // Act & Assert
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync(); // Should not throw
        
        Assert.Equal(1, await context.Invoices.CountAsync());
    }

    #endregion

    #region Referential Integrity Tests

    [Fact]
    public async Task Client_ShouldRestrictDeleteWhenHasCostCenters()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        await context.SaveChangesAsync();

        // Act & Assert
        // EF Core throws InvalidOperationException when trying to remove an entity
        // that has required relationships (DeleteBehavior.Restrict)
        var exception = Assert.Throws<InvalidOperationException>(() => 
            context.Clients.Remove(client));

        Assert.Contains("association", exception.Message?.ToLower());
        Assert.Contains("severed", exception.Message?.ToLower());
    }

    [Fact]
    public async Task CostCenter_ShouldCascadeDeleteToResponsiblePersons()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        var responsiblePerson = new ResponsiblePerson
        {
            CostCenter = costCenter,
            FullName = "John Doe",
            Email = GetUniqueEmail()
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.ResponsiblePersons.Add(responsiblePerson);
        await context.SaveChangesAsync();

        // Act
        context.CostCenters.Remove(costCenter);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await context.ResponsiblePersons.CountAsync()); // Should be cascaded
        Assert.Equal(0, await context.CostCenters.CountAsync());
    }

    [Fact]
    public async Task Invoice_ShouldCascadeDeleteToInvoiceHistories()
    {
        // Arrange
        using var context = CreateCleanContext();

        var client = new Client
        {
            CompanyName = "Test Company",
            TaxId = GetUniqueTaxId(),
            Email = GetUniqueEmail()
        };

        var costCenter = new CostCenter
        {
            Client = client,
            Code = GetUniqueCode(),
            Name = "Test Cost Center"
        };

        // Create InvoiceStatus for the test
        var invoiceStatus = new InvoiceStatus
        {
            Name = "Pending",
            Description = "Pending status for test",
            Color = "#FFA500",
            SortOrder = 1,
            IsActive = true,
            IsFinal = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var invoice = new Invoice
        {
            Client = client,
            CostCenter = costCenter,
            InvoiceStatus = invoiceStatus,
            InvoiceNumber = GetUniqueInvoiceNumber(),
            Amount = 100.00m,
            TotalAmount = 100.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.InvoiceStatuses.Add(invoiceStatus);
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        // Now create InvoiceHistory with proper foreign key references
        var invoiceHistory = new InvoiceHistory
        {
            InvoiceId = invoice.Id,
            FromStatusId = invoiceStatus.Id,
            ToStatusId = invoiceStatus.Id,
            Comments = "Initial status",
            ChangedByUserId = "system-user",
            ChangedAt = DateTime.UtcNow
        };

        context.InvoiceHistories.Add(invoiceHistory);
        await context.SaveChangesAsync();

        // Act
        context.Invoices.Remove(invoice);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await context.InvoiceHistories.CountAsync()); // Should be cascaded
        Assert.Equal(0, await context.Invoices.CountAsync());
    }

    #endregion
}