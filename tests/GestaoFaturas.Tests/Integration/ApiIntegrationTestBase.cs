using GestaoFaturas.Api;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestaoFaturas.Tests.Integration;

public class ApiIntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<PostgreSqlFixture>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly PostgreSqlFixture PostgreSqlFixture;

    public ApiIntegrationTestBase(WebApplicationFactory<Program> factory, PostgreSqlFixture postgreSqlFixture)
    {
        PostgreSqlFixture = postgreSqlFixture;
        Factory = factory;
        Client = Factory.CreateClient();
    }
}
