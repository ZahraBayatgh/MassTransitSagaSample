using Contracts.Dtos;
using System;

namespace Contracts.Events
{
    public interface IProductCatalogMessage
    {
        Guid CorrelationId { get; }
        ProductDto Product { get; }
    }
}
