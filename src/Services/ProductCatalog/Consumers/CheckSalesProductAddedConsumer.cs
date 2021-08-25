using Contracts.Dtos;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductCatalog.Data;
using ProductCatalogService.Dtos;
using ProductCatalogService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogService.Consumers
{
    public class CheckSalesProductAddedConsumer : IConsumer<ISalesProductAdded>
    {
        private readonly ILogger<CheckSalesProductAddedConsumer> _logger;
        private readonly ProductCatalogDbContext _context;
        private readonly IProductService _productService;
        public CheckSalesProductAddedConsumer(ILogger<CheckSalesProductAddedConsumer> logger,
            ProductCatalogDbContext context,
            IProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<ISalesProductAdded> context)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Check SalesProductAddContext
                CheckSalesProductAddContext(context);

                // Get and Check product in db
                var getProduct = await _productService.GetProductByIdAsync(context.Message.Product.ProductId);

                if (context.Message.Product.ProductStatus == ProductStatus.SalesIsOk)
                {
                    var productStatus = (int)ProductStatus.SalesIsOk + (int)getProduct.Value.ProductStatus;
                    UpdateProductStatusRequestDto updateProductStatusRequestDto = new UpdateProductStatusRequestDto(getProduct.Value.Name, productStatus);

                    await _productService.UpdateProductStatusAsync(updateProductStatusRequestDto);
                    context.Message.Product.ProductStatus = ProductStatus.SalesIsOk;
                }
                else
                {
                    // Delete product
                    await _productService.DeleteProductAsync(getProduct.Value.Id);
                    context.Message.Product.ProductStatus = ProductStatus.Pending;

                }


                await context.Publish<IInventoryProductAdded>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Product = context.Message.Product
                });

                transaction.Commit();

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation($"SalesResultIntegrationEvent faild. {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"SalesResultIntegrationEvent with product id failed. Exception detail:{ex.Message}");
                transaction.Rollback();

                throw;
            }
        }

        private static void CheckSalesProductAddContext(ConsumeContext<ISalesProductAdded> context)
        {
            if (context == null)
                throw new ArgumentNullException("SalesProductAddedContext is null.");

            if (context.Message.Product.ProductId <= 0)
                throw new ArgumentNullException("SalesProductAddedContext ProductId is invalid.");
        }
    }
}
