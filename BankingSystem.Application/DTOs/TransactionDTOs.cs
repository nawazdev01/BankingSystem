namespace BankingSystem.Application.DTOs
{
    public class DepositRequest
    {
        public int     AccountId   { get; set; }
        public decimal Amount      { get; set; }
        public string  Description { get; set; }
    }

    public class WithdrawRequest
    {
        public int     AccountId   { get; set; }
        public decimal Amount      { get; set; }
        public string  Description { get; set; }
    }

    public class TransferRequest
    {
        public int     SenderAccountId   { get; set; }
        public int     ReceiverAccountId { get; set; }
        public decimal Amount            { get; set; }
        public string  Description       { get; set; }
    }

    public class TransactionResponse
    {
        public int     Id              { get; set; }
        public string  ReferenceNumber { get; set; }
        public decimal Amount          { get; set; }
        public string  Type            { get; set; }
        public string  Status          { get; set; }
        public string  Description     { get; set; }
        public decimal BalanceAfter    { get; set; }
        public string? FailureReason   { get; set; }
        public DateTime CreatedAt      { get; set; }
        public string  AccountNumber   { get; set; }
        public string? ReceiverAccountNumber { get; set; }
    }
}