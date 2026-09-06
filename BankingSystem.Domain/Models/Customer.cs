using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Models
{
    public class Customer
    {
        public int      Id             { get; set; }
        public string   FirstName      { get; set; }
        public string   LastName       { get; set; }
        public string   Email          { get; set; }
        public string   PhoneNumber    { get; set; }
        public string   Address        { get; set; }
        public DateTime DateOfBirth    { get; set; }
        public string   CustomerNumber { get; set; }
        public string   Username       { get; set; }
        public string   PasswordHash   { get; set; }
        public string   NationalId     { get; set; }
        public bool     IsKYCVerified  { get; set; } = false;
        public bool     IsActive       { get; set; } = true;
        public DateTime CreatedAt      { get; set; }

        // Navigation properties
        public List<Account>     Accounts     { get; set; } = new();
        public List<Loan>        Loans        { get; set; } = new();
    }
}