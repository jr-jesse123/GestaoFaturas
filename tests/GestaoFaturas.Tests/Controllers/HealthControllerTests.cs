using FluentAssertions;
using GestaoFaturas.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFaturas.Tests.Controllers;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public void Get_ReturnsHealthyStatus()
    {
        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        var response = okResult.Value!.GetType().GetProperty("Status")?.GetValue(okResult.Value);
        response.Should().Be("Healthy");
    }

    [Fact]
    public void Get_ReturnsCurrentTimestamp()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var result = _controller.Get();

        // Assert
        var after = DateTime.UtcNow;
        
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var timestamp = okResult!.Value!.GetType().GetProperty("Timestamp")?.GetValue(okResult.Value) as DateTime?;
        
        timestamp.Should().NotBeNull();
        timestamp.Should().BeAfter(before.AddSeconds(-1));
        timestamp.Should().BeBefore(after.AddSeconds(1));
    }
}