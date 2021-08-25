namespace InventoryService.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public InventoryType InventoryTransactionType { get; set; }
        public int Count { get; set; }

    }
    public enum InventoryType
    {
        In = 1,
        Out = 2
    }
}
