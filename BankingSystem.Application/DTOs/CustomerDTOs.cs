namespace BankingSystem.Application.DTOs
{
    public class RegisterCustomerRequest
    {
        public string   FirstName   { get; set; }
        public string   LastName    { get; set; }
        public string   Email       { get; set; }
        public string   PhoneNumber { get; set; }
        public string   Address     { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string   Username    { get; set; }
        public string   Password    { get; set; }
        public string   NationalId  { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string PhoneNumber { get; set; }
        public string Address     { get; set; }
        public string Email       { get; set; }
    }

    public class CustomerLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CustomerResponse
    {
        public int      Id             { get; set; }
        public string   FirstName      { get; set; }
        public string   LastName       { get; set; }
        public string   FullName       { get; set; }
        public string   Email          { get; set; }
        public string   PhoneNumber    { get; set; }
        public string   Address        { get; set; }
        public DateTime DateOfBirth    { get; set; }
        public int      Age            { get; set; }
        public string   CustomerNumber { get; set; }
        public bool     IsKYCVerified  { get; set; }
        public bool     IsActive       { get; set; }
        public DateTime CreatedAt      { get; set; }
    }
}