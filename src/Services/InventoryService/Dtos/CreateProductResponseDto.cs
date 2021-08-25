namespace InventoryService.Dtos
{
    public class CreateProductResponseDto
    {

        public CreateProductResponseDto(int productId, string productName = null)
        {
            ProductId = productId;
            ProductName = productName;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
