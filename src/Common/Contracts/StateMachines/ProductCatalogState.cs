using Automatonymous;
using Contracts.Dtos;
using MassTransit.RedisSagas;
using System;

namespace Contracts.StateMachines
{
    public class ProductCatalogState : SagaStateMachineInstance, IVersionedSaga
    {
        public ProductCatalogState(Guid correlationId)
        {
            CorrelationId = correlationId;
            Product = new ProductDto();
        }
        public State CurrentState { get; set; }

        public Guid CorrelationId { get; set; }
        public ProductDto Product { get; set; }
        public int Version { get; set; }
    }
}
