using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProductCatalogService.Consumers;
using ProductCatalogService.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProductCatalog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await SeedData.Seed();
            CreateHostBuilder(args)
               .Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();
            });
    }
}
