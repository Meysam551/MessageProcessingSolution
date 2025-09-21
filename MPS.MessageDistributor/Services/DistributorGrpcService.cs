
using System.Threading.Channels;
using Distributor.Grpc;
using Grpc.Core;

namespace MPS.MessageDistributor;

/// <summary>
/// gRPC service responsible for handling bi-directional communication with message processors.
/// Implements the DistributorServiceBase generated from distributor.proto.
/// </summary>
public class DistributorGrpcService : DistributorService.DistributorServiceBase
{
    /// <summary>
    /// Internal channel to act as a message queue.
    /// Messages are written here and read by connected clients.
    /// </summary>
    private readonly Channel<QueueMessage> _queueChannel = Channel.CreateUnbounded<QueueMessage>();

    /// <summary>
    /// Bi-directional gRPC stream.
    /// Handles incoming IntroMessage from clients and streams QueueMessage back to them.
    /// </summary>
    /// <param name="requestStream">Incoming stream of IntroMessage from client.</param>
    /// <param name="responseStream">Outgoing stream of QueueMessage to client.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task Connect(
        IAsyncStreamReader<IntroMessage> requestStream,
        IServerStreamWriter<QueueMessage> responseStream,
        ServerCallContext context)
    {
        // Task to read incoming IntroMessages asynchronously
        var readTask = Task.Run(async () =>
        {
            await foreach (var introMsg in requestStream.ReadAllAsync())
            {
                // Initial processing or logging of client connection
                Console.WriteLine($"Client connected: {introMsg.Id}, Type: {introMsg.Type}");
            }
        });

        // Stream messages from internal queue to client
        await foreach (var msg in _queueChannel.Reader.ReadAllAsync())
        {
            await responseStream.WriteAsync(msg);
        }

        // Ensure the reading task completes before exiting
        await readTask;
    }

    /// <summary>
    /// Adds a message to the internal queue.
    /// Acts like enqueuing a message in a message queue.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    public void EnqueueMessage(QueueMessage message)
    {
        _queueChannel.Writer.TryWrite(message);
    }

    /// <summary>
    /// Simple HealthCheck gRPC method.
    /// Returns the status of the service with IsEnabled, NumberOfActiveClients, and ExpirationTime.
    /// </summary>
    /// <param name="request">The health check request containing system info.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>A Task containing HealthResponse.</returns>
    public override Task<HealthResponse> CheckHealth(HealthRequest request, ServerCallContext context)
    {
        var response = new HealthResponse
        {
            // Indicates the service is enabled
            IsEnabled = true,

            // Number of currently active clients (can be dynamic in real implementation)
            NumberOfActiveClients = 3,

            // Expiration timestamp in ISO 8601 format
            ExpirationTime = DateTime.UtcNow.AddMinutes(10).ToString("O")
        };
        return Task.FromResult(response);
    }
}

