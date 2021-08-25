namespace ProductCatalog.Dtos
{
    public class CreateProductResponseDto
    {
        public CreateProductResponseDto(int productId, string name, int initialHand)
        {
            ProductId = productId;
            Name = name;
            InitialHand = initialHand;
        }

        public int ProductId { get; private set; }
        public string Name { get; private set; }

        public int InitialHand { get; private set; }
    }
}
