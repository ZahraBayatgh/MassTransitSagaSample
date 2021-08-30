using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AzureProcessor.Data
{
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
