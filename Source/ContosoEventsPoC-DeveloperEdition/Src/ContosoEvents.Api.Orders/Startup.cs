using System;
using System.IO;
using ContosoEvents.Api.Orders.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace ContosoEvents.Api.Orders
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (hostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IOrdersProvider, FakeOrdersProvider>();
                services.AddSingleton<IQueueService, FakeQueueService>();
            }
            else
            {
                services.AddSingleton<IOrdersProvider, CosmosDbOrdersProvider>();
                services.AddSingleton<IQueueService, AzureQueueService>();
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Contoso Events API - Orders", Version = "v1" });

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "ContosoEvents.Api.Orders.xml");
                c.IncludeXmlComments(xmlPath);
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso Events API V1"); });

            app.UseMvc();
        }
    }
}