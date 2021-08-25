using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesService.Models;
using System;
using System.Threading.Tasks;

namespace SalesService.Data
{
    public class SeedData
    {
        private static IServiceScope GenerateServiceScope()
        {
            var serviceCollection = new ServiceCollection();

            // ICollection
            var configurationBuilder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddJsonFile("appsettings.Development.json", true)
              .AddEnvironmentVariables();

            var config = configurationBuilder.Build();
            serviceCollection.AddSingleton<IConfiguration>(config);

            // DbContext
            serviceCollection.AddDbContext<SaleDbContext>(option =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("ConnectionString Missing!");
                    throw new InvalidProgramException("Missing Connection String");
                }
                option.UseSqlServer(connectionString);
            });

            return serviceCollection.BuildServiceProvider().CreateScope();
        }

        public static async Task Seed()
        {
            using (var serviceScope = GenerateServiceScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var context = serviceProvider.GetService<SaleDbContext>();
                context.Database.EnsureCreated();

                Console.WriteLine("Database Created");


                var product = await context.Products.FirstOrDefaultAsync(x => x.Name == "Mouse");
                if (product == null)
                {
                    product = new Product()
                    {
                        Name = "Mouse",
                        OnHand = 300,
                    };

                    await context.Products.AddAsync(product);
                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Failed to create user");
                }
            };

            Console.WriteLine("Database seeded...");
        }
    }
}
