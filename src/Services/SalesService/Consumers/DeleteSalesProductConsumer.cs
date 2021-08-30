using Contracts.Dtos;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using SalesService.Dtos;
using SalesService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesService.Consumers
{
    public class DeleteSalesProductConsumer : IConsumer<IProductRejected>
    {
        private readonly ILogger<CreateSalesProductConsumer> _logger;
        private readonly IProductService _productService;
        public DeleteSalesProductConsumer(ILogger<CreateSalesProductConsumer> logger,
       IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<IProductRejected> context)
        {
            try
            {
                CheckProductIntegrationEventInstance(context);
                var createProductResponce = await _productService.DeleteProductByNameAsync(context.Message.Product.ProductName);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation($"CreateProductIntegrationEvent is null. Exception detail:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Product {context.Message.Product.ProductName} wan not created. Exception detail:{ex.Message}");

                throw;
            }

        }
        private static void CheckProductIntegrationEventInstance(ConsumeContext<IProductRejected> context)
        {
            if (context == null)
                throw new ArgumentNullException("SalesProduct is null.");

            if (context.Message.Product.Id <= 0)
                throw new ArgumentNullException("SalesProduct ProductId is invalid.");

            if (string.IsNullOrEmpty(context.Message.Product.ProductName))
                throw new ArgumentNullException("SalesProduct ProductName is null.");
        }
    }
}
