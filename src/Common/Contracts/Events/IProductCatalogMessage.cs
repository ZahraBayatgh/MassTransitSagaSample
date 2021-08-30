using Contracts.Data;
using System;

namespace Contracts.Events
{
    public interface IProductCatalogMessage
    {
        Guid CorrelationId { get; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int InitialOnHand { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
}
