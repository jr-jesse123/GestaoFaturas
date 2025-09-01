using GestaoFaturas.Api.Data;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Tests.Integration;

public class MigrationTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public MigrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Database_ShouldApplyAllMigrations()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        
        // Clean slate - drop database and migrate from scratch
        await context.Database.EnsureDeletedAsync();

        // Act
        await context.Database.MigrateAsync();

        // Assert
        Assert.True(await context.Database.CanConnectAsync());
        
        // Verify all tables exist
        var tableNames = new[]
        {
            "clients", "cost_centers", "responsible_persons", 
            "invoices", "invoice_statuses", "invoice_histories"
        };

        foreach (var tableName in tableNames)
        {
            var tableExists = await context.Database.ExecuteSqlRawAsync(
                $"SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}' LIMIT 1"
            ) >= 0;
            Assert.True(tableExists, $"Table {tableName} should exist after migration");
        }
    }

    [Fact]
    public async Task Database_ShouldHaveCorrectIndexes()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Act & Assert - Check for key unique indexes
        var uniqueIndexes = new[]
        {
            ("clients", "ix_clients_tax_id"),
            ("cost_centers", "ix_cost_centers_client_code"),
            ("invoices", "ix_invoices_client_number"),
            ("invoice_statuses", "ix_invoice_statuses_name")
        };

        foreach (var (tableName, indexName) in uniqueIndexes)
        {
            var indexExists = await context.Database.ExecuteSqlRawAsync($@"
                SELECT 1 FROM pg_indexes 
                WHERE tablename = '{tableName}' AND indexname = '{indexName}' 
                LIMIT 1") >= 0;
            Assert.True(indexExists, $"Index {indexName} on table {tableName} should exist");
        }
    }

    [Fact]
    public async Task Database_ShouldHaveCorrectConstraints()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Act & Assert - Check for check constraints
        var checkConstraints = new[]
        {
            "CK_Invoices_Amount_Positive",
            "CK_Invoices_TotalAmount_Positive",
            "CK_Invoices_ServicePeriod_Valid",
            "CK_Invoices_DueDate_Valid"
        };

        foreach (var constraintName in checkConstraints)
        {
            var constraintExists = await context.Database.ExecuteSqlRawAsync($@"
                SELECT 1 FROM information_schema.check_constraints 
                WHERE constraint_name = '{constraintName}' 
                LIMIT 1") >= 0;
            Assert.True(constraintExists, $"Check constraint {constraintName} should exist");
        }
    }

    [Fact]
    public async Task Database_ShouldHaveSeedData()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Act & Assert
        var statusCount = await context.InvoiceStatuses.CountAsync();
        Assert.True(statusCount >= 9, "Should have at least 9 seeded invoice statuses");

        var pendingStatus = await context.InvoiceStatuses
            .FirstOrDefaultAsync(s => s.Name == "Pending");
        Assert.NotNull(pendingStatus);
        Assert.Equal("#FFA500", pendingStatus.Color);
        Assert.Equal(1, pendingStatus.SortOrder);
    }

    [Fact]
    public async Task Database_ShouldSupportBasicCrudOperations()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Clean up any existing data
        context.InvoiceHistories.RemoveRange(context.InvoiceHistories);
        context.Invoices.RemoveRange(context.Invoices);
        context.ResponsiblePersons.RemoveRange(context.ResponsiblePersons);
        context.CostCenters.RemoveRange(context.CostCenters);
        context.Clients.RemoveRange(context.Clients);
        await context.SaveChangesAsync();

        // Act - Create test entities
        var client = new Api.Models.Client
        {
            CompanyName = "Test Company",
            TaxId = DateTime.UtcNow.Ticks.ToString("D14").Substring(0, 14),
            Email = "test@company.com"
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var costCenter = new Api.Models.CostCenter
        {
            ClientId = client.Id,
            Code = "CC001",
            Name = "Test Cost Center"
        };

        context.CostCenters.Add(costCenter);
        await context.SaveChangesAsync();

        var invoiceStatus = await context.InvoiceStatuses.FirstAsync();
        
        var invoice = new Api.Models.Invoice
        {
            ClientId = client.Id,
            CostCenterId = costCenter.Id,
            InvoiceStatusId = invoiceStatus.Id,
            InvoiceNumber = "INV-001",
            Amount = 100.00m,
            TotalAmount = 120.00m,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ServicePeriodStart = DateTime.UtcNow.AddDays(-30),
            ServicePeriodEnd = DateTime.UtcNow
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(client.Id > 0);
        Assert.True(costCenter.Id > 0);
        Assert.True(invoice.Id > 0);

        var retrievedInvoice = await context.Invoices
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .FirstOrDefaultAsync(i => i.Id == invoice.Id);

        Assert.NotNull(retrievedInvoice);
        Assert.Equal("Test Company", retrievedInvoice.Client.CompanyName);
        Assert.Equal("Test Cost Center", retrievedInvoice.CostCenter.Name);
        Assert.Equal(invoiceStatus.Name, retrievedInvoice.InvoiceStatus.Name);
    }

    [Fact]
    public async Task Database_ShouldEnforceReferentialIntegrity()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Clean up existing data
        context.InvoiceHistories.RemoveRange(context.InvoiceHistories);
        context.Invoices.RemoveRange(context.Invoices);
        context.ResponsiblePersons.RemoveRange(context.ResponsiblePersons);
        context.CostCenters.RemoveRange(context.CostCenters);
        context.Clients.RemoveRange(context.Clients);
        await context.SaveChangesAsync();

        // Create client and cost center with responsible person
        var client = new Api.Models.Client
        {
            CompanyName = "Test Company",
            TaxId = DateTime.UtcNow.Ticks.ToString("D14").Substring(0, 14),
            Email = "test@company.com"
        };

        var costCenter = new Api.Models.CostCenter
        {
            Client = client,
            Code = "CC001",
            Name = "Test Cost Center"
        };

        var responsiblePerson = new Api.Models.ResponsiblePerson
        {
            CostCenter = costCenter,
            FullName = "John Doe",
            Email = "john.doe@test.com"
        };

        context.Clients.Add(client);
        context.CostCenters.Add(costCenter);
        context.ResponsiblePersons.Add(responsiblePerson);
        await context.SaveChangesAsync();

        // Act & Assert - Try to delete client (should be restricted)
        context.Clients.Remove(client);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());
        
        Assert.Contains("foreign key", exception.InnerException?.Message?.ToLower());

        // Reset context
        context.Entry(client).State = EntityState.Unchanged;

        // Act & Assert - Delete cost center should cascade to responsible person
        context.CostCenters.Remove(costCenter);
        await context.SaveChangesAsync();

        var remainingPersons = await context.ResponsiblePersons.CountAsync();
        Assert.Equal(0, remainingPersons); // Should be cascaded
    }
}