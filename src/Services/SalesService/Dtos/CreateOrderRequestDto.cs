namespace SalesService.Dtos
{
    public class CreateOrderRequestDto
    {
        public CreateOrderRequestDto(int customerId)
        {
            CustomerId = customerId;
        }

        public int CustomerId { get; set; }
    }
}
