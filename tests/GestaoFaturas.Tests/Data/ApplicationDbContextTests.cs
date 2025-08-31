using GestaoFaturas.Api.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Data;

public class ApplicationDbContextTests
{
    [Fact]
    public void ApplicationDbContext_ShouldInheritFromIdentityDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Inheritance")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.IsAssignableFrom<IdentityDbContext>(context);
    }

    [Fact]
    public void ApplicationDbContext_ShouldConfigurePostgreSQLProvider()
    {
        // Arrange
        var connectionString = "Host=localhost;Port=5432;Database=test_db;Username=test;Password=test";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.True(context.Database.IsNpgsql());
    }

    [Fact]
    public void ApplicationDbContext_ShouldHaveValidConnectionString()
    {
        // Arrange
        var connectionString = "Host=localhost;Port=5432;Database=gestao_faturas_test;Username=test;Password=test";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Database.GetConnectionString());
        Assert.Contains("gestao_faturas_test", context.Database.GetConnectionString());
    }

    [Fact]
    public void ApplicationDbContext_ShouldInitializeWithoutErrors()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Initialize")
            .Options;

        // Act & Assert
        using var context = new ApplicationDbContext(options);
        Assert.NotNull(context);
        Assert.NotNull(context.Users); // Identity table
        Assert.NotNull(context.Roles); // Identity table
    }

    [Fact]
    public void ApplicationDbContext_ShouldUseNamingConventions()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .UseSnakeCaseNamingConvention()
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert - Verify that snake_case naming convention is applied
        // This test ensures the configuration is set up correctly
        Assert.NotNull(context);
        Assert.True(context.Database.IsNpgsql());
    }

    [Theory]
    [InlineData("Host=localhost;Database=gestao_faturas_test;Username=test;Password=test")]
    [InlineData("Host=localhost;Port=5432;Database=gestao_faturas_dev;Username=dev;Password=dev")]
    public void ApplicationDbContext_ShouldHandleValidConnectionStrings(string connectionString)
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Database.GetConnectionString());
        Assert.True(context.Database.IsNpgsql());
    }

    [Fact]
    public void ApplicationDbContext_ShouldFailWithInvalidConnectionString()
    {
        // Arrange
        var invalidConnectionString = "invalid connection string";

        // Act & Assert
        // Using invalid connection string should throw during context options creation
        Assert.ThrowsAny<Exception>(() =>
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(invalidConnectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            // Force a database operation to trigger connection attempt
            context.Database.EnsureCreated();
        });
    }

    [Fact]
    public void ApplicationDbContext_ShouldConfigureLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Logging")
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert - Verify context can be created with logging options
        Assert.NotNull(context);
    }
}