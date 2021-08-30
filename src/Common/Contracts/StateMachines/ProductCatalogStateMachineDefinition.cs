using Contracts.Events;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace Contracts.StateMachines
{
    public class ProductCatalogStateMachineDefinition :
        SagaDefinition<ProductCatalogState>
    {
        public ProductCatalogStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ProductCatalogState> sagaConfigurator)
        {
            var partition = endpointConfigurator.CreatePartitioner(16);

            sagaConfigurator.Message<IProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<ISalesProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<IInventoryProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<IProductRejectedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            
            //endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
            //endpointConfigurator.UseInMemoryOutbox();
        }
    }
    //public class ProductStateMachineDefinition :
    //   SagaDefinition<ProductState>
    //{
    //    public ProductStateMachineDefinition()
    //    {
    //        ConcurrentMessageLimit = 10;
    //    }

    //    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ProductState> sagaConfigurator)
    //    {
    //        var partition = endpointConfigurator.CreatePartitioner(16);

    //        sagaConfigurator.Message<IRegisterProductCommand>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
    //        sagaConfigurator.Message<ISalesProductRegisteredEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
    //        //endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
    //        //endpointConfigurator.UseInMemoryOutbox();
    //}
    //}

    }
