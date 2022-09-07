using System;
using System.Threading;
using System.Threading.Tasks;
using Autobarn.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Autobarn.Notifier {
    internal class Program {
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => {
                    var amqp = hostContext.Configuration.GetConnectionString("AutobarnRabbitMqConnectionString");
                    var bus = RabbitHutch.CreateBus(amqp);
                    services.AddSingleton(bus);
                    var hubUrl = hostContext.Configuration["AutobarnSignalRHubUrl"];
                    var hub = new HubConnectionBuilder().WithUrl(hubUrl).Build();
                    services.AddSingleton(hub);
                    services.AddHostedService<Notifier>();
                })
                .Build();
            await host.RunAsync();
        }
    }

    internal class Notifier : IHostedService {
        private readonly ILogger<Notifier> logger;
        private readonly IBus bus;
        private readonly HubConnection hub;

        public Notifier(
            ILogger<Notifier> logger,
            IBus bus,
            HubConnection hub) {
            this.logger = logger;
            this.bus = bus;
            this.hub = hub;
        }

        private const int MAX_CONNECTION_ATTEMPTS = 10;
        private const int DELAY_IN_MILLISECONDS = 5000;
        public async Task StartAsync(CancellationToken cancellationToken) {
            var attempt = 0;
            while (true) {
                try {
                    logger.LogInformation(
                        $"Connecting to SignalR Hub (attempt #{attempt}");
                    await hub.StartAsync();
                    break;
                } catch (Exception) {
                    if (attempt++ >= MAX_CONNECTION_ATTEMPTS) throw;
                    logger.LogWarning(
                        $"Couldn't connect to SignalR Hub - trying again in {DELAY_IN_MILLISECONDS}ms...");
                    Thread.Sleep(DELAY_IN_MILLISECONDS);
                }
            }

            await bus.PubSub.SubscribeAsync<NewVehiclePriceMessage>("autobarn.notifier", HandleNewVehiclePriceMessage);
        }

        private async Task HandleNewVehiclePriceMessage(NewVehiclePriceMessage m) {
            logger.LogInformation(m.ToString());
            const string user = "autobarn.notifier";
            var message = JsonConvert.SerializeObject(m);
            await hub.SendAsync("ThisMethodNameCanBeAnythingWeLike", user, message);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            await hub.StopAsync();
        }
    }
}

