using System;
using System.Threading;
using System.Threading.Tasks;
using Autobarn.Messages;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autobarn.Notifier {
    internal class Program {
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => {
                    var amqp = hostContext.Configuration.GetConnectionString("AutobarnRabbitMqConnectionString");
                    var bus = RabbitHutch.CreateBus(amqp);
                    services.AddSingleton(bus);

                    services.AddHostedService<Notifier>();
                })
                .Build();
            await host.RunAsync();
        }
    }

    internal class Notifier : IHostedService {
        private readonly ILogger<Notifier> logger;
        private readonly IBus bus;

        public Notifier(
            ILogger<Notifier> logger,
            IBus bus
        ) {
            this.logger = logger;
            this.bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await bus.PubSub.SubscribeAsync<NewVehiclePriceMessage>("autobarn.notifier", HandleNewVehiclePriceMessage);

        }

        private void HandleNewVehiclePriceMessage(NewVehiclePriceMessage m) {
            logger.LogInformation(m.ToString());
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;

        }
    }
}

