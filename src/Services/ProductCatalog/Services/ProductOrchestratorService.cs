using Contracts.Dtos;
using Contracts.Events;
using CSharpFunctionalExtensions;
using MassTransit;
using MassTransit.Util;
using Microsoft.Extensions.Logging;
using ProductCatalog.Data;
using ProductCatalogService.Dtos;
using System;
using System.Threading.Tasks;

namespace ProductCatalogService.Services
{
    public class ProductOrchestratorService : IProductOrchestratorService
    {
        private readonly ProductCatalogDbContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<ProductOrchestratorService> _logger;

        public ProductOrchestratorService(ProductCatalogDbContext context,
            IProductService productService,
            ILogger<ProductOrchestratorService> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }
        public async Task<Result<int>> CreateProductAndPublishEvent(CreateProductRequestDto createProductRequestDto, string correlationId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var createProductResponse = await _productService.CreateProductAsync(createProductRequestDto);

                if (createProductResponse.IsSuccess)
                {
                    ProductDto productDto = new ProductDto
                    {
                        Id = createProductResponse.Value.ProductId,
                        InitialOnHand = createProductRequestDto.InitialHand,
                        ProductName = createProductRequestDto.Name,
                        ProductStatus = ProductStatus.Pending
                    };
                    // Publish CreateProductIntegrationEvent
                    var bus = CreateBust();
                    await bus.Publish<IProductAddedEvent>(new
                    {
                        CorrelationId = Guid.NewGuid(),
                        Product = productDto
                    });

                    transaction.Commit();
                }

                return Result.Success(createProductResponse.Value.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Add {createProductRequestDto.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure<int>($"Add {createProductRequestDto.Name} product failed.");
            }

        }
        private static IBus CreateBust()
        {

            var rabbitHost = new Uri("rabbitmq://localhost");
            var user = "guest";
            var password = "guest";

            var bus = Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                configurator.Host(rabbitHost, h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

            });

            TaskUtil.Await(() => bus.StartAsync());
            return bus;
        }
    }
}
