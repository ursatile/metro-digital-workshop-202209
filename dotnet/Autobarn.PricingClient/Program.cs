using System;
using System.Threading.Tasks;
using Autobarn.PricingEngine;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Autobarn.PricingClient {
    internal class Program {
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => {
                    var amqp = hostContext.Configuration.GetConnectionString("AutobarnRabbitMqConnectionString");
                    var bus = RabbitHutch.CreateBus(amqp);
                    services.AddSingleton(bus);

                    var grpcUrl = hostContext.Configuration["AutobarnPricingServerUrl"];
                    services.AddGrpcClient<Pricer.PricerClient>(options => options.Address = new Uri(grpcUrl));

                    services.AddHostedService<PricingClientWorker>();
                })
                .Build();
            await host.RunAsync();
        }
    }
}
