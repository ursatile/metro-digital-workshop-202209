using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase {
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger) {
        _logger = logger;
    }

    static int count = 0;

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
        var name = $"{request.LastName}, {request.FirstName}";
        Console.WriteLine($"Got a HelloRequest for {name}");
        var message = request.Language switch {
            "en-GB" => $"Good morning, {name}",
            "ro-RO" => $"Salut, {name}",
            "en-AU" => $"G'day, {name}",
            "en-US" => $"Howdy, {name}!",
            "uk-UA" => $"Привіт, {name}!",
            "jp-JP" => $"こんにちは, {name}",
            _ => $"//TODO: localise for {request.Language} {name}"
        };
        var result = Task.FromResult(new HelloReply {
            Message = $"{message}. You were request number {++count}"
        });
        Console.WriteLine($"Replied to request {count} ({message})");
        return result;
    }
}
