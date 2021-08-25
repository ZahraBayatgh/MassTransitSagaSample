namespace ProductCatalog.Dtos
{
    public class CreateProductRequestDto
    {
        public CreateProductRequestDto(string name, string photo, int initialHand)
        {
            Name = name;
            Photo = photo;
            InitialHand = initialHand;
        }

        public string Name { get; set; }
        public string Photo { get; set; }
        public int InitialHand { get; set; }
    }
}
