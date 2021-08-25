namespace ProductCatalogService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
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
