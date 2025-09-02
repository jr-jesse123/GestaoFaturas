using GestaoFaturas.Api;
using GestaoFaturas.Api.Data;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Integration;

public class ApiIntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<PostgreSqlFixture>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly PostgreSqlFixture PostgreSqlFixture;

    public ApiIntegrationTestBase(WebApplicationFactory<Program> factory, PostgreSqlFixture postgreSqlFixture)
    {
        PostgreSqlFixture = postgreSqlFixture;
        
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development"); // Use Development to enable health endpoints
            
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Override the connection string with the test container connection string
                config.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("ConnectionStrings:gestaoFaturas", postgreSqlFixture.ConnectionString),
                    // Disable OpenTelemetry in tests
                    new KeyValuePair<string, string?>("OTEL_EXPORTER_OTLP_ENDPOINT", ""),
                    // Disable service discovery in tests  
                    new KeyValuePair<string, string?>("ServiceDiscovery:Enabled", "false")
                });
            });
            
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add simplified DbContext configuration for tests
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(postgreSqlFixture.ConnectionString)
                           .UseSnakeCaseNamingConvention()
                           .EnableSensitiveDataLogging()
                           .EnableDetailedErrors();
                });
            });
            
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });
        });

        Client = Factory.CreateClient();
    }
}
