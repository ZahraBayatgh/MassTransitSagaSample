namespace SalesService.Dtos
{
    public class CreateCustomerRequestDto
    {

        public CreateCustomerRequestDto(string firstName, string lastName = null)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
