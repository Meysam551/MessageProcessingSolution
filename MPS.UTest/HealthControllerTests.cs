using System;
using Microsoft.AspNetCore.Mvc;
using MPS.ManagementSystem.Controllers;
using MPS.Shared.Contracts;
using Xunit;

namespace MPS.UTest;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        // Arrange: ساخت کنترلر قبل از هر تست
        _controller = new HealthController();
    }

    [Fact]
    public void CheckHealth_ShouldReturn_ValidResponse()
    {
        // Arrange
        var request = new HealthRequest{ Id = Guid.NewGuid(), SystemTime = DateTime.UtcNow, NumberOfConnectedClients = 5 };

        // Act
        var result = _controller.CheckHealth(request);

        // Assert: بررسی اینکه نتیجه OkObjectResult باشد
        var okResult = Assert.IsType<OkObjectResult>(result);

        // بررسی اینکه Value از نوع HealthResponse باشد
        var response = Assert.IsType<HealthResponse>(okResult.Value);

        // بررسی فیلدها
        Assert.True(response.IsEnabled, "IsEnabled should always be true");
        Assert.InRange(response.NumberOfActiveClients, 0, 5);

        // بررسی ExpirationTime
        Assert.True(response.ExpirationTime > DateTime.UtcNow, "ExpirationTime should be in the future");
        Assert.True(response.ExpirationTime < DateTime.UtcNow.AddMinutes(11), "ExpirationTime should be roughly 10 minutes from now");
    }
}
