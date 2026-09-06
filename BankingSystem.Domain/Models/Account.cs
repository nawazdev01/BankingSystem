using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Models
{
    public class Account
    {
        public int           Id             { get; set; }
        public string        AccountNumber  { get; set; }
        public AccountType   Type           { get; set; }
        public AccountStatus Status         { get; set; } = AccountStatus.Active;
        public decimal       Balance        { get; protected set; }
        public DateTime      OpenedDate     { get; set; }
        public decimal       DailyLimit     { get; set; } = 10000m;

        // Type specific — nullable because not all accounts need them
        public decimal?  InterestRate  { get; set; }  // Savings, Fixed
        public decimal?  OverdraftLimit{ get; set; }  // Current only
        public DateTime? MaturityDate  { get; set; }  // Fixed Deposit only

        // Foreign key
        public int      CustomerId { get; set; }
        public Customer Customer   { get; set; }

        // Navigation
        public List<Transaction> Transactions { get; set; } = new();
        public List<Loan>        Loans        { get; set; } = new();

        // Domain methods — business operations on account
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive");
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Cannot deposit to inactive account");
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive");
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Cannot withdraw from inactive account");

            decimal effectiveLimit = Type == AccountType.Current
                ? Balance + (OverdraftLimit ?? 0)
                : Balance;

            if (amount > effectiveLimit)
                throw new InvalidOperationException("Insufficient funds");

            Balance -= amount;
        }
    }
}