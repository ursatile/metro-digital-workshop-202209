using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Graylog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => {
        services.AddHostedService<ExampleService>();
    })
    .UseSerilog(ConfigureLogger)
    .Build();

host.Run();

static void ConfigureLogger(HostBuilderContext host, LoggerConfiguration log) {
    log.MinimumLevel.Debug();
    log.WriteTo.Console();
    log.WriteTo.Graylog(
        new GraylogSinkOptions { 
            HostnameOrAddress = "localhost", 
            Port = 12201
    });
    log.Enrich.WithProcessName();
}