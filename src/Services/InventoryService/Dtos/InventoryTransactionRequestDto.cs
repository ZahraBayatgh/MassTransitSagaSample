using InventoryService.Models;

namespace InventoryService.Dtos
{
    public class InventoryTransactionRequestDto
    {

        public InventoryTransactionRequestDto(int productId, int count, InventoryType type)
        {
            ProductId = productId;
            Count = count;
            Type = type;
        }
        public int ProductId { get; private set; }
        public int Count { get; private set; }
        public InventoryType Type { get; private set; }
    }
}
