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
    public class CreateSalesProductConsumer : IConsumer<ICreateSalesProduct>
    {
        private readonly ILogger<CreateSalesProductConsumer> _logger;
        private readonly IProductService _productService;
        public CreateSalesProductConsumer(ILogger<CreateSalesProductConsumer> logger,
       IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<ICreateSalesProduct> context)
        {
            try
            {
                CheckCreateProductIntegrationEventInstance(context);

                // Create product
                var createProductRequestDto = new CreateProductRequestDto
                {
                    Name = context.Message.Product.ProductName,
                    Count = context.Message.Product.InitialOnHand
                };
                var createProductResponce = await _productService.CreateProductAsync(createProductRequestDto);


                bool createProductStatus = createProductResponce.IsSuccess ? true : false;
                await PublishResult(context, createProductStatus);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation($"CreateProductIntegrationEvent is null. Exception detail:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Product {context.Message.Product.ProductName} wan not created. Exception detail:{ex.Message}");

                await PublishResult(context, false);

                throw;
            }

        }

        private async Task PublishResult(ConsumeContext<ICreateSalesProduct> context, bool createProductStatus)
        {
            if (createProductStatus)
                context.Message.Product.ProductStatus = ProductStatus.SalesIsOk;

            await context.Publish<ISalesProductAdded>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Product = context.Message.Product
            });
        }

        private static void CheckCreateProductIntegrationEventInstance(ConsumeContext<ICreateSalesProduct> context)
        {
            if (context == null)
                throw new ArgumentNullException("CreateSalesProduct is null.");

            if (context.Message.Product.Id <= 0)
                throw new ArgumentNullException("CreateSalesProduct ProductId is invalid.");

            if (string.IsNullOrEmpty(context.Message.Product.ProductName))
                throw new ArgumentNullException("CreateSalesProduct ProductName is null.");
        }
    }
}
