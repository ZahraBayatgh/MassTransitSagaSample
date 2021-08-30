using Contracts.Data;
using Contracts.Events;
using InventoryService.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using InventoryService.Dtos;
using InventoryService.Data;
using InventoryService.Models;

namespace ProductCatalogService.Consumers
{
    public class CreateInventoryProductConsumer : IConsumer<ICreateInventoryProductEvent>
    {
        private readonly ILogger<CreateInventoryProductConsumer> _logger;
        private readonly InventoryDbContext _context;
        private readonly IInventoryTransactionService _inventoryTransactionService;
        private readonly IProductService _productService;
        public CreateInventoryProductConsumer(ILogger<CreateInventoryProductConsumer> logger,
            IInventoryTransactionService inventoryTransactionService,
            IProductService productService,
            InventoryDbContext context)
        {
            _logger = logger;
            _context = context;
            _inventoryTransactionService = inventoryTransactionService;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<ICreateInventoryProductEvent> context)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Check SalesProductAddContext
                CheckSalesProductAddContext(context);
                bool createProductStatus = false;
                if (context.Message.ProductStatus == ProductStatus.SalesIsOk)
                {
                    // Create product
                    var ProductRequestDto = new ProductRequestDto
                    {
                        ProductName = context.Message.ProductName
                    };
                    var createProductResponce = await _productService.CreateProductAsync(ProductRequestDto);

                    if (createProductResponce.IsFailure)
                        throw new Exception("CreateProductIntegrationEvent is failure.");

                    // Initial InventoryTransactionRequestDto
                    var InventoryTransactionDto = new InventoryTransactionRequestDto(createProductResponce.Value.ProductId, context.Message.InitialOnHand, InventoryType.In);

                    // Add InventoryTransaction
                    var inventoryTransactionResponse = await _inventoryTransactionService.CreateInventoryTransactionAsync(InventoryTransactionDto);

                    createProductStatus = createProductResponce.IsSuccess ? true : false;
                }
              
                await PublishResult(context, createProductStatus);
                transaction.Commit();

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation($"SalesResultIntegrationEvent faild. {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"SalesResultIntegrationEvent with {context.Message.ProductId} product id failed. Exception detail:{ex.Message}");
                transaction.Rollback();

                throw;
            }
        }
        private async Task PublishResult(ConsumeContext<ICreateInventoryProductEvent> context, bool createProductStatus)
        {
            if (createProductStatus)
                context.Message.ProductStatus = ProductStatus.InventoryIsOk;

            await context.Publish<IInventoryProductAddedEvent>(new
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId,
                ProductName = context.Message.ProductName,
                InitialOnHand = context.Message.InitialOnHand,
                ProductStatus = context.Message.ProductStatus
            });
        }
        private static void CheckSalesProductAddContext(ConsumeContext<ICreateInventoryProductEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException("SalesProductAddedContext is null.");

            if (context.Message.ProductId <= 0)
                throw new ArgumentNullException("SalesProductAddedContext ProductId is invalid.");
        }
    }
}
