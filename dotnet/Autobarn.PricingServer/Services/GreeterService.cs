using System;
using System.Threading.Tasks;
using Autobarn.PricingEngine;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Autobarn.PricingServer.Services {
    public class PricerService : Pricer.PricerBase {
        private readonly ILogger<PricerService> logger;
        public PricerService(ILogger<PricerService> logger) {
            this.logger = logger;
        }

        public override Task<PriceReply> GetPrice(PriceRequest request, ServerCallContext context) {
            logger.LogDebug("Received a GetPrice with PriceRequest", request);
            var (price, currencyCode) = GetPrice(request);
            var reply = new PriceReply {
                Price = price,
                CurrencyCode = currencyCode
            };
            logger.LogDebug("Sending a PriceReply", reply);
            return Task.FromResult(reply);
        }

        private (int, string) GetPrice(PriceRequest request) {
            if (request.ManufacturerName.Contains("ford", StringComparison.InvariantCultureIgnoreCase))
                return (20000, "GBP");
            if (request.Color.Equals("brown", StringComparison.InvariantCultureIgnoreCase))
                return (150000, "EUR");
            if (request.ManufacturerName.Contains("dacia", StringComparison.InvariantCultureIgnoreCase))
                return (50000 + request.Year * 1000, "RON");
            return (10000 + request.Year * 100, "EUR");
        }
    }
}
