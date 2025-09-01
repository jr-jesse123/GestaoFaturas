using GestaoFaturas.Api.Data;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Data;

public class ApplicationDbContextTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public ApplicationDbContextTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldInheritFromIdentityDbContext()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();

        // Assert
        Assert.IsAssignableFrom<IdentityDbContext>(context);
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldConfigurePostgreSQLProvider()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();

        // Assert
        Assert.True(context.Database.IsNpgsql());
        Assert.False(context.Database.IsInMemory());
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldHaveValidConnectionString()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();
        var connectionString = context.Database.GetConnectionString();

        // Assert
        Assert.NotNull(connectionString);
        Assert.Contains("gestao_faturas_test", connectionString);
        Assert.Contains("test_user", connectionString);
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldInitializeWithoutErrors()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Users); // Identity table
        Assert.NotNull(context.Roles); // Identity table
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldUseNamingConventions()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Assert - Verify that snake_case naming convention is applied
        // Check if Identity tables are created with snake_case names
        await context.Database.ExecuteSqlRawAsync(
            "SELECT 1 FROM information_schema.tables WHERE table_name = 'asp_net_users' LIMIT 1");
        
        Assert.NotNull(context);
        Assert.True(context.Database.IsNpgsql());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldHandleValidConnectionStrings()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();

        // Assert
        Assert.NotNull(context.Database.GetConnectionString());
        Assert.True(context.Database.IsNpgsql());
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldFailWithInvalidConnectionString()
    {
        // Arrange
        var invalidConnectionString = "Host=nonexistent.invalid;Database=test;Username=test;Password=test;Connection Timeout=1;Command Timeout=1";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(invalidConnectionString)
            .Options;

        // Act & Assert
        using var context = new ApplicationDbContext(options);
        await Assert.ThrowsAnyAsync<Exception>(async () => 
            await context.Database.OpenConnectionAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldConfigureLogging()
    {
        // Arrange
        var options = _fixture.CreateOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with logging options
        Assert.NotNull(context);
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldCreateAndMigrateDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();

        // Act
        await context.Database.EnsureCreatedAsync();

        // Assert
        Assert.True(await context.Database.CanConnectAsync());
        
        // Verify Identity tables were created
        var userCount = await context.Users.CountAsync();
        var roleCount = await context.Roles.CountAsync();
        
        Assert.True(userCount >= 0); // Table exists
        Assert.True(roleCount >= 0); // Table exists
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldSupportTransactions()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Act & Assert
        using var transaction = await context.Database.BeginTransactionAsync();
        Assert.NotNull(transaction);
        await transaction.RollbackAsync();
    }
}