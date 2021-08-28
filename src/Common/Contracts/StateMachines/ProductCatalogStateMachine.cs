using Automatonymous;
using Automatonymous.Binders;
using Common.Logging;
using Contracts.Dtos;
using Contracts.Events;
using System;
using System.Threading.Tasks;
using SagaState = Automatonymous.State;

namespace Contracts.StateMachines
{
    public class ProductCatalogStateMachine : MassTransitStateMachine<ProductCatalogState>
    {
        private readonly ILog logger;

        public ProductCatalogStateMachine()
        {
            logger = LogManager.GetLogger<ProductCatalogStateMachine>();

            InstanceState(x => x.CurrentState);
            State(() => Pending);
            ConfigureCorrelationIds();
            
            Initially(SetProductCatalogAddedHandler());
            During(SalesSubmited, SetSalesProductAddedHandler(),  SetInventoryAddedHandler());

            SetCompletedWhenFinalized();

        }
        private EventActivityBinder<ProductCatalogState, IProductCatalogAdded> SetProductCatalogAddedHandler() =>
            When(ProductCatalogAdded).Then(c => UpdateSagaState(c.Instance, c.Data.Product))
                                  .Then(c => logger.Info($"Product Catalog Added to {c.Data.CorrelationId} received"))
                                    .ThenAsync(c => this.SendCommand<ICreateSalesProduct>("rabbitmq://localhost/sagas-demo-sale", c))
                                      .TransitionTo(SalesSubmited);
                                     


        private EventActivityBinder<ProductCatalogState, ISalesProductAdded> SetSalesProductAddedHandler() =>
           When(SalesProductAdded).Then(c => this.UpdateSagaState(c.Instance, c.Data.Product))
                                      .Then(c => this.logger.Info($"Sales Product Added to {c.Data.CorrelationId} received"))
                                        .ThenAsync(c => this.SendCommand<ICreateInventoryProduct>("rabbitmq://localhost/sagas-demo-inventory", c))
                                     .TransitionTo(InventorySubmited);
        private EventActivityBinder<ProductCatalogState, IInventoryProductAdded> SetInventoryAddedHandler()=>
          When(InventoryProductAdded).Then(c => this.UpdateSagaState(c.Instance, c.Data.Product))
                                  .Then(c => this.logger.Info($"Inventory Product Added to {c.Data.CorrelationId} received"))
                                  .TransitionTo(Completed)
                                   .Finalize(); 
    
        private void ConfigureCorrelationIds()
        {
            Event(() => ProductCatalogAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            Event(() => SalesProductAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            Event(() => InventoryProductAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));

        }
        private void UpdateSagaState(ProductCatalogState state, ProductDto productDto)
        {
            state.Product.Id = productDto.Id;
            state.Product.ProductName = productDto.ProductName;
            state.Product.InitialOnHand = productDto.InitialOnHand;
            state.Product.ProductStatus = productDto.ProductStatus;
        }
        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProductCatalogState, IProductCatalogMessage> context)
           where TCommand : class, IProductCatalogMessage
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(endpointKey));
            await sendEndpoint.Send<TCommand>(new
            {
                context.Data.CorrelationId,
                context.Data.Product,
            });
        }
        public Event<IProductCatalogAdded> ProductCatalogAdded { get; private set; }
        public Event<ISalesProductAdded> SalesProductAdded { get; private set; }
        public Event<IInventoryProductAdded> InventoryProductAdded { get; private set; }

        public SagaState Pending { get; private set; }
        public SagaState SalesSubmited { get; private set; }
        public SagaState InventorySubmited { get; private set; }
        public SagaState Completed { get; private set; }
    }
}
