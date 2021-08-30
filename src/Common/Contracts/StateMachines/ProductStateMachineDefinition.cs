using Contracts.Events;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace Contracts.StateMachines
{
    public class ProductStateMachineDefinition :
        SagaDefinition<ProductState>
    {
        public ProductStateMachineDefinition()
        {
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ProductState> sagaConfigurator)
        {
            var partition = endpointConfigurator.CreatePartitioner(16);

            sagaConfigurator.Message<IProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<ISalesProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<IInventoryProductAddedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            sagaConfigurator.Message<IProductRejectedEvent>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            
        }
    }
    }
