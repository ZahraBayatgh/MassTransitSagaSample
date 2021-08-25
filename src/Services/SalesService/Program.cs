using Autofac.Extensions.DependencyInjection;
using MassTransit.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SalesService.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using MassTransit;
using SalesService.Consumers;
using SalesService.Services;

namespace SalesService
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

