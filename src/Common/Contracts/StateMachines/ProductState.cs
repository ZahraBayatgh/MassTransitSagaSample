using Automatonymous;
using Contracts.Data;
using Dapper.Contrib.Extensions;
using MassTransit.RedisSagas;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.StateMachines
{
    public class ProductState : SagaStateMachineInstance, IVersionedSaga
    {

        public ProductState(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public string CurrentState { get; set; }
        [ExplicitKey]
        public Guid CorrelationId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int InitialOnHand { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public int Version { get; set; }
    }
}
