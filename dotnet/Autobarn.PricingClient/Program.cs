using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<PricingClientWorker> logger;

        public PricingClientWorker(ILogger<PricingClientWorker> logger) {
            this.logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken) {
            logger.LogCritical("THIS IS A CRITICAL PROBLEM");
            logger.LogError("This went wrong but somebody probably tried again and it worked!");
            logger.LogWarning("Something went wrong but nobody noticed");
            logger.LogInformation("Started the pricing client!");
            logger.LogDebug("This is really detailed debugging information");
            logger.LogTrace("This is even more detailed debugging information");
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Stopping the pricing client!");
            return Task.CompletedTask;
        }
    }

}
