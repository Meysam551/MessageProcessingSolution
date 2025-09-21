

namespace MPS.Shared.Contracts;
public record HealthRequest
{
    public Guid Id { get; init; }
    public DateTime SystemTime { get; init; }
    public int NumberOfConnectedClients { get; init; }
}
