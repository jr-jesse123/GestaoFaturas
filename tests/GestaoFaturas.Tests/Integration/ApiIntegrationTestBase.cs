using GestaoFaturas.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GestaoFaturas.Tests.Integration;

public class ApiIntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    public ApiIntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                // Override services for testing if needed
                // For example, replace database with in-memory database
            });
            
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });
        });

        Client = Factory.CreateClient();
    }
}