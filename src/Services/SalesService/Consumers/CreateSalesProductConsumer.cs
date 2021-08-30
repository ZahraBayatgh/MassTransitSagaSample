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
    public class CreateSalesProductConsumer : IConsumer<ICreateSalesProductCommand>
    {
        private readonly ILogger<CreateSalesProductConsumer> _logger;
        private readonly IProductService _productService;
        public CreateSalesProductConsumer(ILogger<CreateSalesProductConsumer> logger,
       IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<ICreateSalesProductCommand> context)
        {
            try
            {
                CheckCreateProductIntegrationEventInstance(context);

                // Create product
                var createProductRequestDto = new CreateProductRequestDto
                {
                    Name = context.Message.ProductName,
                    Count = context.Message.InitialOnHand
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
                _logger.LogInformation($"Product {context.Message.ProductName} wan not created. Exception detail:{ex.Message}");

                await PublishResult(context, false);

                throw;
            }

        }

        private async Task PublishResult(ConsumeContext<ICreateSalesProductCommand> context, bool createProductStatus)
        {
            if (createProductStatus)
                context.Message.ProductStatus = ProductStatus.SalesIsOk;

            await context.Publish<ISalesProductAddedEvent>(new
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId,
                ProductName = context.Message.ProductName,
                InitialOnHand = context.Message.InitialOnHand,
                ProductStatus = context.Message.ProductStatus
            });
        }

        private static void CheckCreateProductIntegrationEventInstance(ConsumeContext<ICreateSalesProductCommand> context)
        {
            if (context == null)
                throw new ArgumentNullException("CreateSalesProduct is null.");

            if (context.Message.ProductId <= 0)
                throw new ArgumentNullException("CreateSalesProduct ProductId is invalid.");

            if (string.IsNullOrEmpty(context.Message.ProductName))
                throw new ArgumentNullException("CreateSalesProduct ProductName is null.");
        }
    }
}
