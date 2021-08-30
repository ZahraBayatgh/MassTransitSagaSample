using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureProcessor.Data
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
            serviceCollection.AddDbContext<ProductDbContext>(option =>
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

        public static void Seed()
        {
            using (var serviceScope = GenerateServiceScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var context = serviceProvider.GetService<ProductDbContext>();
                context.Database.EnsureCreated();

                Console.WriteLine("Database Created");
            };

            Console.WriteLine("Database seeded...");
        }
    }
}
