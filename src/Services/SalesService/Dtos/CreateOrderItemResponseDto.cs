namespace SalesService.Dtos
{
    public class CreateOrderItemResponseDto
    {

        public CreateOrderItemResponseDto(int orderItemId, int orderId, string name, int quantity)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            ProductName = name;
            Quantity = quantity;
        }

        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
