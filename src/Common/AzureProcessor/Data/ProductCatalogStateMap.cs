using Contracts.StateMachines;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureProcessor.Data
{
    public class ProductCatalogStateMap :
    SagaClassMap<ProductCatalogState>
    {
        protected override void Configure(EntityTypeBuilder<ProductCatalogState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);

            entity.Property(x => x.Version);
        }
    }
}
