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
        private readonly IConfiguration configuration;

        public Notifier(
            ILogger<Notifier> logger,
            IBus bus,
            HubConnection hub,
            IConfiguration configuration) {
            this.logger = logger;
            this.bus = bus;
            this.hub = hub;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            logger.LogInformation($"Connecting to SignalR using {configuration["AutobarnSignalRHubUrl"]}")
            await hub.StartAsync();
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

