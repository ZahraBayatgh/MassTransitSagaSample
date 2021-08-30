using Contracts.StateMachines;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureProcessor.Data
{
    public class ProductStateMap :
    SagaClassMap<ProductState>
    {
        protected override void Configure(EntityTypeBuilder<ProductState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);

            entity.Property(x => x.Version);
        }
    }
}
