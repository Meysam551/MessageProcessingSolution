
namespace MPS.Shared.Contracts;
public record HealthResponse
{
    public bool IsEnabled { get; init; }
    public int NumberOfActiveClients { get; init; }
    public DateTime ExpirationTime { get; init; }
}