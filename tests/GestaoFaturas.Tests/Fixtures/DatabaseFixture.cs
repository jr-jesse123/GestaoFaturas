using GestaoFaturas.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoFaturas.Tests.Fixtures;

public class DatabaseFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public DatabaseFixture()
    {
        var services = new ServiceCollection();

        // Use in-memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        _serviceProvider = services.BuildServiceProvider();
    }

    public ApplicationDbContext CreateContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}