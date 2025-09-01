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
            var sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = @tableName";
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);
            
            await context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await context.Database.CloseConnectionAsync();
            
            var tableCount = Convert.ToInt32(result);
            Assert.True(tableCount > 0, $"Table {tableName} should exist after migration");
        }
    }

    [Fact]
    public async Task Database_ShouldHaveCorrectIndexes()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // Act & Assert - Check for key unique indexes (PostgreSQL converts names to lowercase)
        var uniqueIndexes = new[]
        {
            ("clients", "ix_clients_tax_id"),
            ("cost_centers", "ix_cost_centers_client_code"),
            ("invoices", "ix_invoices_client_number"),
            ("invoice_statuses", "ix_invoice_statuses_name")
        };

        foreach (var (tableName, indexName) in uniqueIndexes)
        {
            var sql = "SELECT COUNT(*) FROM pg_indexes WHERE tablename = @tableName AND indexname = @indexName";
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            
            var tableParam = command.CreateParameter();
            tableParam.ParameterName = "@tableName";
            tableParam.Value = tableName;
            command.Parameters.Add(tableParam);
            
            var indexParam = command.CreateParameter();
            indexParam.ParameterName = "@indexName";
            indexParam.Value = indexName;
            command.Parameters.Add(indexParam);
            
            await context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await context.Database.CloseConnectionAsync();
            
            var indexCount = Convert.ToInt32(result);
            Assert.True(indexCount > 0, $"Index {indexName} on table {tableName} should exist");
        }
    }

    [Fact]
    public async Task Database_ShouldHaveCorrectConstraints()
    {
        // Arrange
        using var context = _fixture.CreateMigrationContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        // First, let's see what constraints actually exist
        var sql = @"SELECT cc.constraint_name 
                    FROM information_schema.check_constraints cc
                    JOIN information_schema.constraint_column_usage ccu 
                      ON cc.constraint_name = ccu.constraint_name
                    WHERE ccu.table_name = 'invoices'";
                    
        var actualConstraints = new List<string>();
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        
        await context.Database.OpenConnectionAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            actualConstraints.Add(reader.GetString(0));
        }
        await context.Database.CloseConnectionAsync();
        
        // Check for the constraints we expect (using actual names found)
        var expectedConstraints = new[]
        {
            "CK_Invoices_Amount_Positive",
            "CK_Invoices_TotalAmount_Positive", 
            "CK_Invoices_ServicePeriod_Valid",
            "CK_Invoices_DueDate_Valid"
        };

        foreach (var expectedConstraint in expectedConstraints)
        {
            var found = actualConstraints.Any(ac => 
                string.Equals(ac, expectedConstraint, StringComparison.OrdinalIgnoreCase));
            Assert.True(found, $"Check constraint {expectedConstraint} should exist. Found: [{string.Join(", ", actualConstraints)}]");
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
        // EF Core throws InvalidOperationException when trying to remove entity with required relationships
        var exception = Assert.Throws<InvalidOperationException>(() => 
            context.Clients.Remove(client));
        
        Assert.Contains("association", exception.Message?.ToLower());
        Assert.Contains("severed", exception.Message?.ToLower());

        // Reset context
        context.Entry(client).State = EntityState.Unchanged;

        // Act & Assert - Delete cost center should cascade to responsible person
        context.CostCenters.Remove(costCenter);
        await context.SaveChangesAsync();

        var remainingPersons = await context.ResponsiblePersons.CountAsync();
        Assert.Equal(0, remainingPersons); // Should be cascaded
    }
}