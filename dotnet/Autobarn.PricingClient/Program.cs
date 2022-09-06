using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Autobarn.PricingClient {
    internal class Program {
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<PricingClientWorker>();
                })
                .Build();
            await host.RunAsync();
        }
    }

    internal class PricingClientWorker : IHostedService {
        public Task StartAsync(CancellationToken cancellationToken) {
            Console.WriteLine("Started the pricing client!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            Console.WriteLine("Stopping the pricing client!");
            return Task.CompletedTask;
        }
    }

}
