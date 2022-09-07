using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autobarn.Messages;
using Autobarn.PricingEngine;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autobarn.PricingClient
{
    internal class PricingClientWorker : IHostedService {
        private readonly ILogger<PricingClientWorker> logger;
        private readonly IBus bus;
        private readonly Pricer.PricerClient grpc;

        public PricingClientWorker(
            ILogger<PricingClientWorker> logger,
            IBus bus,
            Pricer.PricerClient grpc
        ) {
            this.logger = logger;
            this.bus = bus;
            this.grpc = grpc;
        }
        public async Task StartAsync(CancellationToken cancellationToken) {
            //logger.LogCritical("THIS IS A CRITICAL PROBLEM");
            //logger.LogError("This went wrong but somebody probably tried again and it worked!");
            //logger.LogWarning("Something went wrong but nobody noticed");
            //logger.LogInformation("Started the pricing client!");
            //logger.LogDebug("This is really detailed debugging information");
            //logger.LogTrace("This is even more detailed debugging information");
            await bus.PubSub.SubscribeAsync<NewVehicleMessage>("autobarn.pricingclient", HandleNewVehicleMessage);
        }

        public async Task HandleNewVehicleMessage(NewVehicleMessage m) {
            logger.LogInformation(m.ToString());
            var priceRequest = new PriceRequest {
                Color = m.Color,
                ManufacturerName = m.ManufacturerName,
                ModelName = m.ModelName,
                Year = m.Year
            };
            var priceReply = await grpc.GetPriceAsync(priceRequest);
            logger.LogInformation($"Got a price: {priceReply.Price} {priceReply.CurrencyCode}");
            var nvpm = new NewVehiclePriceMessage() {
                Color = m.Color,
                ManufacturerName = m.ManufacturerName,
                ModelName = m.ModelName,
                Year = m.Year,
                Registration = m.Registration,
                ListedAt = m.ListedAt,
                Price = priceReply.Price,
                Currency = priceReply.CurrencyCode
            };
            await this.bus.PubSub.PublishAsync(nvpm);
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Stopping the pricing client!");
            return Task.CompletedTask;
        }
    }
}
