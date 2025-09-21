
using Distributor.Grpc;
using Grpc.Core;
using Grpc.Net.Client;

namespace MPS.MessageProcessor;

public class ProcessorWorker
{
    private readonly string _address;


    public ProcessorWorker(string address)
    {
        _address = address;
    }


    public async Task RunAsync(CancellationToken ct)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new DistributorService.DistributorServiceClient(channel);


        using var call = client.Connect();


        // send intro
        await call.RequestStream.WriteAsync(new IntroMessage { Id = GenerateUniqueId(), Type = "RegexEngine" });


        // read messages and process
        var readTask = Task.Run(async () =>
        {
            await foreach (var msg in call.ResponseStream.ReadAllAsync(ct))
            {
                // analyze message
                var result = Analyze(msg, new Dictionary<string, string>());
                // TODO: send result back (depending on proto shape)
            }
        }, ct);


        await readTask;
    }


    private string GenerateUniqueId()
    {
        // example: base on MAC/host
        return Guid.NewGuid().ToString();
    }


    private ProcessedMessage Analyze(QueueMessage msg, Dictionary<string, string> rules)
    {
        var regexResults = new Dictionary<string, bool>();
        foreach (var kv in rules)
        {
            regexResults[kv.Key] = System.Text.RegularExpressions.Regex.IsMatch(msg.Message, kv.Value);
        }


        return new ProcessedMessage
        {
            Id = msg.Id,
            Engine = "RegexEngine",
            MessageLength = msg.Message.Length,
            IsValid = true,
            RegexResults = { }
        };
    }
}
