namespace ProductCatalogService.Dtos
{
    public class UpdateProductStatusRequestDto
    {
        public UpdateProductStatusRequestDto(string name, int productStatus)
        {
            Name = name;
            ProductStatus = productStatus;
        }

        public string Name { get; private set; }
        public int ProductStatus { get; private set; }
    }
}
