using Contracts.Dtos;
using System;

namespace Contracts.Events
{
    [Serializable]

    public class ProductCatalogProcessed : IProductCatalogProcessed
    {
        public ProductCatalogProcessed(Guid correlationId, ProductDto productDto)
        {
            CorrelationId = correlationId;
            Product = productDto;
        }
        public Guid CorrelationId { get; }

        public ProductDto Product { get; set; }

    }
}
