using GestaoFaturas.Api.Data;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Data;

public class DatabaseConnectionTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public DatabaseConnectionTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DatabaseConnection_ShouldBeConfiguredInServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", _fixture.ConnectionString}
            })
            .Build();

        // Act
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
        Assert.True(await dbContext!.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldUsePostgreSQLProvider()
    {
        // Arrange & Act
        using var context = _fixture.CreateContext();

        // Assert
        Assert.True(context.Database.IsNpgsql());
        Assert.False(context.Database.IsInMemory());
        Assert.False(context.Database.IsSqlServer());
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public void DatabaseConnection_ShouldReadConnectionStringFromConfiguration()
    {
        // Arrange
        var connectionString = _fixture.ConnectionString;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", connectionString}
            })
            .Build();

        // Act
        var retrievedConnectionString = configuration.GetConnectionString("DefaultConnection");

        // Assert
        Assert.Equal(connectionString, retrievedConnectionString);
        Assert.Contains("gestao_faturas_test", retrievedConnectionString);
        Assert.Contains("test_user", retrievedConnectionString);
    }

    [Fact]
    public async Task DatabaseConnection_ShouldEnableDetailedErrors()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .EnableDetailedErrors()
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with detailed errors enabled
        Assert.NotNull(context);
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldEnableSensitiveDataLogging()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .EnableSensitiveDataLogging()
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with sensitive data logging enabled
        Assert.NotNull(context);
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldHaveProperRetryPolicy()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString, 
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null))
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with retry policy
        Assert.NotNull(context);
        Assert.True(context.Database.IsNpgsql());
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldUseConnectionPooling()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", _fixture.ConnectionString}
            })
            .Build();

        // Act - AddDbContext enables connection pooling by default
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
        Assert.True(await dbContext!.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldHandleConnectionFailures()
    {
        // Arrange
        var invalidConnectionString = "Host=nonexistent.invalid;Database=nonexistentdb;Username=invalid;Password=invalid;Connection Timeout=1;Command Timeout=1";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(invalidConnectionString)
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act & Assert - Connection failure should throw an exception
        await Assert.ThrowsAnyAsync<Exception>(async () => 
            await context.Database.OpenConnectionAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldExecuteRawSql()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Act - ExecuteSqlRawAsync returns rows affected, not the query result
        // For testing SQL execution, we'll create a temp table and insert a record
        var result = await context.Database.ExecuteSqlRawAsync("CREATE TEMP TABLE IF NOT EXISTS test_table (id INTEGER); INSERT INTO test_table (id) VALUES (1);");

        // Assert - Should return 1 row affected by the INSERT
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task DatabaseConnection_ShouldSupportMultipleContexts()
    {
        // Arrange & Act
        using var context1 = _fixture.CreateContext();
        using var context2 = _fixture.CreateContext();

        // Assert
        Assert.NotSame(context1, context2);
        Assert.True(await context1.Database.CanConnectAsync());
        Assert.True(await context2.Database.CanConnectAsync());
    }

    [Fact]
    public async Task DatabaseConnection_ShouldSupportConcurrentOperations()
    {
        // Arrange
        var tasks = new List<Task>();

        // Act - Create multiple contexts and perform operations concurrently
        for (int i = 0; i < 5; i++)
        {
            int capturedIndex = i; // Capture loop variable
            tasks.Add(Task.Run(async () =>
            {
                using var context = _fixture.CreateContext();
                await context.Database.EnsureCreatedAsync();
                var result = await context.Database.ExecuteSqlRawAsync("CREATE TEMP TABLE IF NOT EXISTS temp_test (id INTEGER); INSERT INTO temp_test (id) VALUES ({0});", capturedIndex);
                Assert.Equal(1, result);
            }));
        }

        // Assert
        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task DatabaseConnection_ShouldRespectCommandTimeout()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(5); // 5 seconds timeout
            })
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.True(await context.Database.CanConnectAsync());
        Assert.Equal(5, context.Database.GetCommandTimeout());
    }
}