using Automatonymous;
using Automatonymous.Binders;
using Common.Logging;
using Contracts.Data;
using Contracts.Events;
using System;
using System.Threading.Tasks;
using SagaState = Automatonymous.State;

namespace Contracts.StateMachines
{
    public class ProductStateMachine : MassTransitStateMachine<ProductState>
    {
        private readonly ILog logger;

        public ProductStateMachine()
        {
                logger = LogManager.GetLogger<ProductStateMachine>();

                InstanceState(x => x.CurrentState);
                State(() => Pending);
                ConfigureCorrelationIds();

                Initially(SetProductCatalogAddedHandler());
                During(SalesSubmited, SetSalesProductAddedHandler());
                During(InventorySubmited, SetInventoryAddedHandler());
                During(Completed, SetInventoryAddedHandler(),
                 SetProductRejectedHandler());

            SetCompletedWhenFinalized();

        }
        private EventActivityBinder<ProductState, IProductAddedEvent> SetProductCatalogAddedHandler() =>
         When(ProductCatalogAdded).Then(c => UpdateSagaState(c.Instance, c.Data.ProductId,c.Data.ProductName,c.Data.InitialOnHand,c.Data.ProductStatus))
                                  .Then(c => logger.Info($"Product Catalog Added to {c.Data.CorrelationId} received"))
                                    .ThenAsync(c => this.SendCommand<ICreateSalesProductCommand>("rabbitmq://localhost/sagas-demo-sale", c))
                                      .TransitionTo(SalesSubmited);

        private EventActivityBinder<ProductState, ISalesProductAddedEvent> SetSalesProductAddedHandler()=>
        When(SalesProductAdded).Then(c => this.UpdateSagaState(c.Instance,c.Data.ProductId, c.Data.ProductName, c.Data.InitialOnHand, c.Data.ProductStatus))
                                      .Then(c => this.logger.Info($"Sales Product Added to {c.Data.CorrelationId} received"))
                                     .TransitionTo(InventorySubmited);
        private EventActivityBinder<ProductState, IInventoryProductAddedEvent> SetInventoryAddedHandler() =>
          When(InventoryProductAdded).Then(c => this.UpdateSagaState(c.Instance, c.Data.ProductId, c.Data.ProductName, c.Data.InitialOnHand, c.Data.ProductStatus))
                                  .Then(c => this.logger.Info($"Inventory Product Added to {c.Data.CorrelationId} received"))
                                .TransitionTo(Completed).Finalize();
        private EventActivityBinder<ProductState, IProductRejectedEvent> SetProductRejectedHandler() =>
              When(ProductRejected).Then(c => this.UpdateSagaState(c.Instance, c.Data.ProductId, c.Data.ProductName, c.Data.InitialOnHand, c.Data.ProductStatus))
                                      .Then(c => this.logger.Info($" Product Rejected to {c.Data.CorrelationId} received"))
                                    .TransitionTo(Rejected)
                                         .Finalize();
        private void ConfigureCorrelationIds()
        {
            Event(() => ProductCatalogAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            Event(() => SalesProductAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            Event(() => InventoryProductAdded, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            Event(() => ProductRejected, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            
        }
        private void UpdateSagaState(ProductState state, int productId,string productName,int initialOnHand, ProductStatus productStatus)
        {
            state.ProductId = productId;
            state.ProductName = productName;
            state.InitialOnHand = initialOnHand;
            state.ProductStatus = productStatus;
        }
        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProductState, IProductCatalogMessage> context)
           where TCommand : class, IProductCatalogMessage
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(endpointKey));
            await sendEndpoint.Send<TCommand>(new
            {
                context.Data.CorrelationId,
                ProductId = context.Data.ProductId,
                ProductName = context.Data.ProductName,
                InitialOnHand = context.Data.InitialOnHand,
                ProductStatus = context.Data.ProductStatus
            });
        }
        public Event<IProductAddedEvent> ProductCatalogAdded { get; private set; }
        public Event<ISalesProductAddedEvent> SalesProductAdded { get; private set; }
        public Event<IInventoryProductAddedEvent> InventoryProductAdded { get; private set; }
        public Event<IProductRejectedEvent> ProductRejected { get; private set; }
        
        public SagaState Pending { get; private set; }
        public SagaState SalesSubmited { get; private set; }
        public SagaState InventorySubmited { get; private set; }
        public SagaState Completed { get; private set; }
        public SagaState Rejected { get; private set; }
    }
}
