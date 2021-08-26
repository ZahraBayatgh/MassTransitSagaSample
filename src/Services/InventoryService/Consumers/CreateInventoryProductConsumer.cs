using Contracts.Dtos;
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
    public class CreateInventoryProductConsumer : IConsumer<ICreateInventoryProduct>
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
        public async Task Consume(ConsumeContext<ICreateInventoryProduct> context)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Check SalesProductAddContext
                CheckSalesProductAddContext(context);
                bool createProductStatus = false;
                if (context.Message.Product.ProductStatus == ProductStatus.SalesIsOk)
                {
                    // Create product
                    var ProductRequestDto = new ProductRequestDto
                    {
                        ProductName = context.Message.Product.ProductName
                    };
                    var createProductResponce = await _productService.CreateProductAsync(ProductRequestDto);

                    if (createProductResponce.IsFailure)
                        throw new Exception("CreateProductIntegrationEvent is failure.");

                    // Initial InventoryTransactionRequestDto
                    var InventoryTransactionDto = new InventoryTransactionRequestDto(createProductResponce.Value.ProductId, context.Message.Product.InitialOnHand, InventoryType.In);

                    // Add InventoryTransaction
                    var inventoryTransactionResponse = await _inventoryTransactionService.CreateInventoryTransactionAsync(InventoryTransactionDto);

                    createProductStatus = createProductResponce.IsSuccess ? true : false;
                    context.Message.Product.ProductStatus = ProductStatus.InventoryIsOk;

                }
                else
                {
                    context.Message.Product.ProductStatus = ProductStatus.Failed;
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
                _logger.LogInformation($"SalesResultIntegrationEvent with {context.Message.Product.ProductId} product id failed. Exception detail:{ex.Message}");
                transaction.Rollback();

                throw;
            }
        }
        private async Task PublishResult(ConsumeContext<ICreateInventoryProduct> context, bool createProductStatus)
        {
            await context.Publish<IInventoryProductAdded>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Product = context.Message.Product
            });
        }
        private static void CheckSalesProductAddContext(ConsumeContext<ICreateInventoryProduct> context)
        {
            if (context == null)
                throw new ArgumentNullException("SalesProductAddedContext is null.");

            if (context.Message.Product.ProductId <= 0)
                throw new ArgumentNullException("SalesProductAddedContext ProductId is invalid.");
        }
    }
}
