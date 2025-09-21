using Microsoft.AspNetCore.Mvc;
using MPS.Shared.Contracts;

namespace MPS.ManagementSystem.Controllers;

/// <summary>
/// Controller responsible for handling health check requests.
/// </summary>
[ApiController]
[Route("api/module/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Handles POST requests to check the system health.
    /// </summary>
    /// <param name="request">
    /// A <see cref="HealthRequest"/> object containing:
    /// <list type="bullet">
    /// <item><description>Id: A temporary GUID representing the client/system.</description></item>
    /// <item><description>SystemTime: Current system time of the client.</description></item>
    /// <item><description>NumberOfConnectedClients: Number of clients currently connected.</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// A <see cref="HealthResponse"/> object containing:
    /// <list type="bullet">
    /// <item><description>IsEnabled: Always true, indicates the system is enabled.</description></item>
    /// <item><description>NumberOfActiveClients: Random number between 0 and 5, representing currently active clients.</description></item>
    /// <item><description>ExpirationTime: UTC timestamp set to current time + 10 minutes, indicates when this status expires.</description></item>
    /// </list>
    /// </returns>
    [HttpPost]
    public IActionResult CheckHealth([FromBody] HealthRequest request)
    {
        // Create a HealthResponse object
        var response = new HealthResponse
        {
            /// <summary>Indicates whether the service is enabled. Always true.</summary>
            IsEnabled = true,
            /// <summary>Number of active clients, randomly generated between 0 and 5.</summary>
            NumberOfActiveClients = Random.Shared.Next(0, 6),
            /// <summary>Expiration time for this health status, current UTC time + 10 minutes.</summary>
            ExpirationTime = DateTime.UtcNow.AddMinutes(10)
        };


        return Ok(response);
    }
}
