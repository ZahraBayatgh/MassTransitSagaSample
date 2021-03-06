using Contracts.Data;
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
    public class DeleteSalesProductConsumer : IConsumer<IProductRejectedEvent>
    {
        private readonly ILogger<CreateSalesProductConsumer> _logger;
        private readonly IProductService _productService;
        public DeleteSalesProductConsumer(ILogger<CreateSalesProductConsumer> logger,
       IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<IProductRejectedEvent> context)
        {
            try
            {
                CheckProductIntegrationEventInstance(context);
                var createProductResponce = await _productService.DeleteProductByNameAsync(context.Message.ProductName);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation($"CreateProductIntegrationEvent is null. Exception detail:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Product {context.Message.ProductName} wan not created. Exception detail:{ex.Message}");

                throw;
            }

        }
        private static void CheckProductIntegrationEventInstance(ConsumeContext<IProductRejectedEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException("SalesProduct is null.");

            if (context.Message.ProductId <= 0)
                throw new ArgumentNullException("SalesProduct ProductId is invalid.");

            if (string.IsNullOrEmpty(context.Message.ProductName))
                throw new ArgumentNullException("SalesProduct ProductName is null.");
        }
    }
}
