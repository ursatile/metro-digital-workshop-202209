using EasyNetQ;
using Messages;

const string AMQP = "amqps://dllepypl:rRHZ7tOnM-dRXtm-cRMRJHRNsSNr7_jq@hefty-blond-bloodhound.rmq4.cloudamqp.com/dllepypl";
using var bus = RabbitHutch.CreateBus(AMQP);
int n = 0;
Console.WriteLine("Press a key to publish a message");
while(true) {
    var g = new Greeting {
        Number = n++
    };
    bus.PubSub.Publish(g);
    Console.WriteLine($"Published {g}");
    Console.ReadKey(true);
}
