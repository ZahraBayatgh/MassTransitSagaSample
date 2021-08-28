using Contracts.StateMachines;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace AzureProcessor.Maps
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
    public class ProductCatalogDbContext :
    SagaDbContext
    {
        public ProductCatalogDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new ProductCatalogStateMap(); }
        }
    }
}
