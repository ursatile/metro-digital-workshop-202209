﻿using EasyNetQ;
using Messages;

const string AMQP = "amqps://dllepypl:rRHZ7tOnM-dRXtm-cRMRJHRNsSNr7_jq@hefty-blond-bloodhound.rmq4.cloudamqp.com/dllepypl";
using var bus = RabbitHutch.CreateBus(AMQP);

Console.WriteLine("Listening for messages. Press enter to exit");
bus.PubSub.Subscribe<Greeting>("SUBSCRIPTION_ID", greeting => {
    Console.WriteLine(greeting);
});

Console.ReadLine();