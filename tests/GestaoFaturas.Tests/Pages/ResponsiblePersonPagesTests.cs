using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GestaoFaturas.Api.Data;
using GestaoFaturas.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GestaoFaturas.Tests.Pages;

public class ResponsiblePersonPagesTests : IClassFixture<WebApplicationFactory<GestaoFaturas.Api.Program>>
{
    private readonly WebApplicationFactory<GestaoFaturas.Api.Program> _factory;

    public ResponsiblePersonPagesTests(WebApplicationFactory<GestaoFaturas.Api.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext configuration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add an in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });
            });
        });
    }

    private async Task<Client> SeedTestClientAsync(HttpClient client)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var testClient = new Client
        {
            CompanyName = "Test Company",
            TaxId = "12345678901234",
            Email = "company@test.com",
            Phone = "+55 11 98765-4321",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Clients.Add(testClient);
        await context.SaveChangesAsync();
        
        return testClient;
    }

    private async Task<ResponsiblePerson> SeedTestResponsiblePersonAsync(HttpClient httpClient, int clientId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var responsiblePerson = new ResponsiblePerson
        {
            ClientId = clientId,
            Name = "John Doe",
            Email = "john.doe@test.com",
            Phone = "+55 11 98765-4321",
            Role = "Finance Manager",
            Department = "Finance",
            IsPrimaryContact = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.ResponsiblePersons.Add(responsiblePerson);
        await context.SaveChangesAsync();
        
        return responsiblePerson;
    }

    [Fact]
    public async Task Get_ResponsiblePersonIndexPage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testClient = await SeedTestClientAsync(client);

        // Act
        var response = await client.GetAsync($"/Clients/ResponsiblePersons?clientId={testClient.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ResponsiblePersonCreatePage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testClient = await SeedTestClientAsync(client);

        // Act
        var response = await client.GetAsync($"/Clients/ResponsiblePersons/Create?clientId={testClient.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ResponsiblePersonEditPage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testClient = await SeedTestClientAsync(client);
        var responsiblePerson = await SeedTestResponsiblePersonAsync(client, testClient.Id);

        // Act
        var response = await client.GetAsync($"/Clients/ResponsiblePersons/Edit?id={responsiblePerson.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ResponsiblePersonDetailsPage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testClient = await SeedTestClientAsync(client);
        var responsiblePerson = await SeedTestResponsiblePersonAsync(client, testClient.Id);

        // Act
        var response = await client.GetAsync($"/Clients/ResponsiblePersons/Details?id={responsiblePerson.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ResponsiblePersonDeletePage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testClient = await SeedTestClientAsync(client);
        var responsiblePerson = await SeedTestResponsiblePersonAsync(client, testClient.Id);

        // Act
        var response = await client.GetAsync($"/Clients/ResponsiblePersons/Delete?id={responsiblePerson.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_ResponsiblePersonIndexPage_WithoutClientId_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Clients/ResponsiblePersons");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ResponsiblePersonEditPage_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Clients/ResponsiblePersons/Edit?id=99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ResponsiblePersonDetailsPage_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Clients/ResponsiblePersons/Details?id=99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ResponsiblePersonDeletePage_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Clients/ResponsiblePersons/Delete?id=99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ResponsiblePersonCreate_WithValidData_RedirectsToIndex()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var testClient = await SeedTestClientAsync(client);

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("ResponsiblePerson.ClientId", testClient.Id.ToString()),
            new KeyValuePair<string, string>("ResponsiblePerson.Name", "Jane Smith"),
            new KeyValuePair<string, string>("ResponsiblePerson.Email", "jane.smith@test.com"),
            new KeyValuePair<string, string>("ResponsiblePerson.Phone", "+55 11 98765-4321"),
            new KeyValuePair<string, string>("ResponsiblePerson.Role", "Finance Director"),
            new KeyValuePair<string, string>("ResponsiblePerson.Department", "Finance"),
            new KeyValuePair<string, string>("ResponsiblePerson.IsPrimaryContact", "false"),
            new KeyValuePair<string, string>("ResponsiblePerson.IsActive", "true")
        });

        // Act
        var response = await client.PostAsync($"/Clients/ResponsiblePersons/Create?clientId={testClient.Id}", formData);

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains($"/Clients/ResponsiblePersons?clientId={testClient.Id}", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Post_ResponsiblePersonEdit_WithValidData_RedirectsToIndex()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var testClient = await SeedTestClientAsync(client);
        var responsiblePerson = await SeedTestResponsiblePersonAsync(client, testClient.Id);

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("ResponsiblePerson.Id", responsiblePerson.Id.ToString()),
            new KeyValuePair<string, string>("ResponsiblePerson.ClientId", testClient.Id.ToString()),
            new KeyValuePair<string, string>("ResponsiblePerson.Name", "John Doe Updated"),
            new KeyValuePair<string, string>("ResponsiblePerson.Email", "john.updated@test.com"),
            new KeyValuePair<string, string>("ResponsiblePerson.Phone", "+55 11 98765-9999"),
            new KeyValuePair<string, string>("ResponsiblePerson.Role", "Finance Director"),
            new KeyValuePair<string, string>("ResponsiblePerson.Department", "Finance"),
            new KeyValuePair<string, string>("ResponsiblePerson.IsPrimaryContact", "true"),
            new KeyValuePair<string, string>("ResponsiblePerson.IsActive", "true")
        });

        // Act
        var response = await client.PostAsync($"/Clients/ResponsiblePersons/Edit?id={responsiblePerson.Id}", formData);

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains($"/Clients/ResponsiblePersons?clientId={testClient.Id}", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Post_ResponsiblePersonDelete_RemovesPersonAndRedirects()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var testClient = await SeedTestClientAsync(client);
        var responsiblePerson = await SeedTestResponsiblePersonAsync(client, testClient.Id);

        // Act
        var response = await client.PostAsync($"/Clients/ResponsiblePersons/Delete?id={responsiblePerson.Id}", new StringContent(""));

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains($"/Clients/ResponsiblePersons?clientId={testClient.Id}", response.Headers.Location?.ToString());

        // Verify deletion
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deletedPerson = await context.ResponsiblePersons.FindAsync(responsiblePerson.Id);
        Assert.Null(deletedPerson);
    }
}