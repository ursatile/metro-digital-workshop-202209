using EasyNetQ;
using Autobarn.Messages;
using System;
using System.Threading.Tasks;

namespace Autobarn.AuditLog {
    class Program {
        const string AMQP = "amqps://dllepypl:rRHZ7tOnM-dRXtm-cRMRJHRNsSNr7_jq@hefty-blond-bloodhound.rmq4.cloudamqp.com/dllepypl";
        public static async Task Main(string[] args) {
            using var bus = RabbitHutch.CreateBus(AMQP);
            await bus.PubSub.SubscribeAsync<NewVehicleMessage>("autobarn.auditlog", Handle);
            Console.WriteLine("Subscribed to NewVehicleMessages...");
            Console.ReadLine();
        }

        private static void Handle(NewVehicleMessage message) {
            Console.WriteLine(message);
        }
    }
}
