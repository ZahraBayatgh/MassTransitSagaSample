using Automatonymous;
using Contracts.Dtos;
using Dapper.Contrib.Extensions;
using MassTransit.RedisSagas;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.StateMachines
{
    public class ProductCatalogState : SagaStateMachineInstance, IVersionedSaga
    {

        public ProductCatalogState(Guid correlationId)
        {
            CorrelationId = correlationId;
            Product = new ProductDto();
        }
        public string CurrentState { get; set; }
        [ExplicitKey]
        public Guid CorrelationId { get; set; }
        public ProductDto Product { get; set; }
        public int Version { get; set; }
    }
}
