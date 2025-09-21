
using System.Threading.Channels;
using Distributor.Grpc;

namespace MPS.MessageDistributor;

/// <summary>
/// Simulates a message queue for testing or development purposes.
/// Generates random messages in the background and provides async reading capabilities.
/// </summary>
public class MessageQueueSimulator
{
    /// <summary>
    /// Internal unbounded channel acting as the message queue.
    /// </summary>
    private readonly Channel<QueueMessage> _channel = Channel.CreateUnbounded<QueueMessage>();

    /// <summary>
    /// Cancellation token source to control background producer loop.
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of <see cref="MessageQueueSimulator"/>.
    /// Starts the background producer loop immediately.
    /// </summary>
    public MessageQueueSimulator()
    {
        // Start the background task that produces messages continuously
        _ = ProduceLoopAsync(_cts.Token);
    }

    /// <summary>
    /// Background loop that produces random messages every 200ms.
    /// </summary>
    /// <param name="ct">Cancellation token to stop the loop gracefully.</param>
    private async Task ProduceLoopAsync(CancellationToken ct)
    {
        var rnd = new Random();
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(200, ct); // Simulate message arrival delay

            // Create a random QueueMessage
            var msg = new QueueMessage
            {
                Id = rnd.Next(1, 10000), // Random message ID
                Sender = "Legal", // Fixed sender for simulation
                Message = Guid.NewGuid().ToString() // Random message content
            };

            // Write the message into the internal channel
            await _channel.Writer.WriteAsync(msg, ct);
        }
    }

    /// <summary>
    /// Reads a message asynchronously from the internal queue.
    /// </summary>
    /// <param name="ct">Cancellation token to cancel the read operation.</param>
    /// <returns>A <see cref="ValueTask{QueueMessage}"/> representing the next message.</returns>
    public ValueTask<QueueMessage> ReadAsync(CancellationToken ct) => _channel.Reader.ReadAsync(ct);

    /// <summary>
    /// Stops the background producer loop and cancels all ongoing operations.
    /// </summary>
    public void Stop() => _cts.Cancel();
}
