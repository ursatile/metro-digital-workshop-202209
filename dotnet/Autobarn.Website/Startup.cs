using Autobarn.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Autobarn.Website.GraphQL.GraphTypes;
using Autobarn.Website.GraphQL.Schemas;
using Autobarn.Website.Hubs;
using EasyNetQ;
using GraphiQl;
using GraphQL;
using GraphQL.Types;
using Microsoft.OpenApi.Models;

namespace Autobarn.Website {
    public class Startup {
        protected virtual string DatabaseMode => Configuration["DatabaseMode"];

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSingleton<IAutobarnDatabase, AutobarnCsvFileDatabase>();

            var amqp = Configuration.GetConnectionString("AutobarnRabbitMqConnectionString");
            var bus = RabbitHutch.CreateBus(amqp);
            services.AddSingleton(bus);

            services.AddSwaggerGen(
                config => {
                    config.SwaggerDoc("v1", new OpenApiInfo() {
                        Title = "Autobarn API",
                        Description = "The world's best API for selling second hand cars!",
                        Contact = new OpenApiContact {
                            Name = "Dylan Beattie",
                            Email = "dylan@ursatile.com",
                            Url = new Uri("https://twitter.com/dylanbeattie"),
                        },
                        License = new OpenApiLicense {
                            Name = "Use under LICX",
                            Url = new Uri("https://example.com/license"),
                        }
                    });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    config.IncludeXmlComments(xmlPath);
                });
            services.AddGraphQL(builder => builder
               .AddNewtonsoftJson()
               .AddSchema<AutobarnSchema>()
               .AddGraphTypes(typeof(VehicleGraphType).Assembly)
           );

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseGraphQL<ISchema>();
            app.UseGraphiQl("/graphiql");

            app.UseEndpoints(endpoints => {
                endpoints.MapHub<AutobarnHub>("/hub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
