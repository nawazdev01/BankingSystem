using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Models
{
    public class Transaction
    {
        public int               Id                { get; set; }
        public string            ReferenceNumber   { get; set; }
        public decimal           Amount            { get; set; }
        public TransactionType   Type              { get; set; }
        public TransactionStatus Status            { get; set; }
        public string            Description       { get; set; }
        public decimal           BalanceAfter      { get; set; }
        public DateTime          CreatedAt         { get; set; }
        public string?           FailureReason     { get; set; }

        // Which account this transaction belongs to
        public int     AccountId { get; set; }
        public Account Account   { get; set; }

        // For transfers — destination account
        public int?     ReceiverAccountId { get; set; }
        public Account? ReceiverAccount   { get; set; }
    }
}