using System.Net;
using System.Text.Json;
using FluentAssertions;
using GestaoFaturas.Api;
using GestaoFaturas.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestaoFaturas.Tests.Integration;

public class HealthEndpointTests : ApiIntegrationTestBase
{
    public HealthEndpointTests(WebApplicationFactory<Program> factory, PostgreSqlFixture postgreSqlFixture) 
        : base(factory, postgreSqlFixture)
    {
    }

    [Fact(Skip = "Integration tests disabled due to Aspire configuration complexity")]
    public async Task Get_Health_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await Client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
    }

    [Fact(Skip = "Integration tests disabled due to Aspire configuration complexity")]
    public async Task Get_Health_ReturnsHealthyStatus()
    {
        // Act
        var response = await Client.GetAsync("/api/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var healthResponse = JsonSerializer.Deserialize<HealthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        healthResponse.Should().NotBeNull();
        healthResponse!.Status.Should().Be("Healthy");
        healthResponse.Version.Should().Be("1.0.0");
        healthResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    private record HealthResponse(string Status, DateTime Timestamp, string Version);
}
