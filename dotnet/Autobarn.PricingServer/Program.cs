using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Graylog;

namespace Autobarn.PricingServer {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .UseSerilog(ConfigureLogger)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.ConfigureKestrel(options => {
                        var pfxPassword = Environment.GetEnvironmentVariable("UrsatilePfxPassword");
                        var https = UseCertIfAvailable(@"D:\Dropbox\workshop.ursatile.com\workshop.ursatile.com.pfx", pfxPassword);
                        options.ListenAnyIP(5002, listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2);
                        options.Listen(IPAddress.Any, 5003, https);
                        options.AllowSynchronousIO = true;
                    });
                    webBuilder.UseStartup<Startup>();
                });

        private static Action<ListenOptions> UseCertIfAvailable(string pfxFilePath, string pfxPassword) {
            if (File.Exists(pfxFilePath)) return listen => listen.UseHttps(pfxFilePath, pfxPassword);
            return listen => listen.UseHttps();
        }

        static void ConfigureLogger(HostBuilderContext host, LoggerConfiguration log) {
            log.MinimumLevel.Debug();
            log.WriteTo.Console();
            log.WriteTo.Graylog(
                new GraylogSinkOptions {
                    HostnameOrAddress = "workshop.ursatile.com",
                    Port = 12201
                });
            log.Enrich.WithProcessName();
        }
    }
}
