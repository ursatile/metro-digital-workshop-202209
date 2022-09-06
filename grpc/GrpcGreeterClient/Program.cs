using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeterClient;
using System.Diagnostics;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://workshop.ursatile.com:5003");
var client = new Greeter.GreeterClient(channel);
while(true) {
    Console.WriteLine("Press any key to send a gRPC request...");
    Console.ReadKey();
    var s = Stopwatch.StartNew();
    var reply = await client.SayHelloAsync(
                    new HelloRequest { Name = "Dylan", Language="ro-RO" });
    Console.WriteLine("Greeting: " + reply.Message);
    Console.WriteLine($"That took {s.ElapsedMilliseconds}ms");
}