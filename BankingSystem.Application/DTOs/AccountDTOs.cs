using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTOs
{
    public class CreateAccountRequest
    {
        public int         CustomerId    { get; set; }
        public AccountType Type          { get; set; }
        public decimal     InitialDeposit{ get; set; }

        // Optional — only for specific account types
        public decimal?  InterestRate  { get; set; }
        public decimal?  OverdraftLimit{ get; set; }
        public DateTime? MaturityDate  { get; set; }
    }

    public class AccountResponse
    {
        public int           Id            { get; set; }
        public string        AccountNumber { get; set; }
        public string        Type          { get; set; }
        public string        Status        { get; set; }
        public decimal       Balance       { get; set; }
        public DateTime      OpenedDate    { get; set; }
        public decimal       DailyLimit    { get; set; }
        public decimal?      InterestRate  { get; set; }
        public decimal?      OverdraftLimit{ get; set; }
        public DateTime?     MaturityDate  { get; set; }
        public string        CustomerName  { get; set; }
        public string        CustomerNumber{ get; set; }
    }
}