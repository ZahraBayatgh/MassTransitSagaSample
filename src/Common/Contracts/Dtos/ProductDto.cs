using System;

namespace Contracts.Dtos
{
    [Serializable]
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int InitialOnHand { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
    public enum ProductStatus
    {
        Pending = 0,
        SalesIsOk = 2,
        InventoryIsOk = 4,
        Completed = 6
    }
}
