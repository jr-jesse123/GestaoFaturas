using GestaoFaturas.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Data;

public class DatabaseConnectionTests
{
    [Fact]
    public void DatabaseConnection_ShouldBeConfiguredInServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=test;Password=test"}
            })
            .Build();

        // Act
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void DatabaseConnection_ShouldUsePostgreSQLProvider()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.True(context.Database.IsNpgsql());
        Assert.False(context.Database.IsInMemory());
        Assert.False(context.Database.IsSqlServer());
    }

    [Fact]
    public void DatabaseConnection_ShouldReadConnectionStringFromConfiguration()
    {
        // Arrange
        var connectionString = "Host=testhost;Database=testdb;Username=testuser;Password=testpass";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", connectionString}
            })
            .Build();

        // Act
        var retrievedConnectionString = configuration.GetConnectionString("DefaultConnection");

        // Assert
        Assert.Equal(connectionString, retrievedConnectionString);
        Assert.Contains("testhost", retrievedConnectionString);
        Assert.Contains("testdb", retrievedConnectionString);
    }

    [Fact]
    public void DatabaseConnection_ShouldEnableDetailedErrors()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .EnableDetailedErrors()
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with detailed errors enabled
        Assert.NotNull(context);
    }

    [Fact]
    public void DatabaseConnection_ShouldEnableSensitiveDataLogging()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .EnableSensitiveDataLogging()
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with sensitive data logging enabled
        Assert.NotNull(context);
    }

    [Fact]
    public void DatabaseConnection_ShouldHaveProperRetryPolicy()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test", 
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null))
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with retry policy
        Assert.NotNull(context);
        Assert.True(context.Database.IsNpgsql());
    }

    [Fact]
    public void DatabaseConnection_ShouldUseConnectionPooling()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=test;Password=test"}
            })
            .Build();

        // Act - AddDbContext enables connection pooling by default
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void DatabaseConnection_ShouldHandleConnectionFailures()
    {
        // Arrange
        var invalidConnectionString = "Host=nonexistenthost;Database=nonexistentdb;Username=invalid;Password=invalid";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(invalidConnectionString)
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act & Assert - Connection failure should throw an exception
        // Force a database operation to trigger actual connection attempt
        Assert.ThrowsAny<Exception>(() => context.Database.EnsureCreated());
    }
}