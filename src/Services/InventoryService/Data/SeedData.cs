using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InventoryService.Data
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
            serviceCollection.AddDbContext<InventoryDbContext>(option =>
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
                var context = serviceProvider.GetService<InventoryDbContext>();
                context.Database.EnsureCreated();

                Console.WriteLine("Database Created");

                // Check product exist
                var product = await context.Products.FirstOrDefaultAsync(x => x.Name == "Mouse");
                if (product == null)
                {
                    product = new Product()
                    {
                        Name = "Mouse",
                    };
                    await context.Products.AddAsync(product);

                    var inventoryTransaction = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        InventoryTransactionType = InventoryType.In,
                        Count = 30,
                    };
                    await context.AddAsync(inventoryTransaction);

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
