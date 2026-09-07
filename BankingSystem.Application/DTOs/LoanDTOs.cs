using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTOs
{
    public class ApplyLoanRequest
    {
        public int      CustomerId { get; set; }
        public int      AccountId  { get; set; }
        public LoanType Type       { get; set; }
        public decimal  LoanAmount { get; set; }
        public int      TermMonths { get; set; }
        public string   Purpose    { get; set; }
    }

    public class ApproveLoanRequest
    {
        public decimal ApprovedAmount { get; set; }
        public decimal InterestRate   { get; set; }
    }

    public class RejectLoanRequest
    {
        public string RejectionReason { get; set; }
    }

    public class LoanRepaymentRequest
    {
        public int     LoanId { get; set; }
        public decimal Amount { get; set; }
    }

    public class LoanResponse
    {
        public int      Id               { get; set; }
        public string   LoanNumber       { get; set; }
        public string   Type             { get; set; }
        public string   Status           { get; set; }
        public decimal  LoanAmount       { get; set; }
        public decimal? ApprovedAmount   { get; set; }
        public decimal  InterestRate     { get; set; }
        public int      TermMonths       { get; set; }
        public decimal? MonthlyPayment   { get; set; }
        public decimal  AmountRepaid     { get; set; }
        public decimal  RemainingAmount  { get; set; }
        public string   Purpose          { get; set; }
        public DateTime ApplicationDate  { get; set; }
        public DateTime? ApprovalDate    { get; set; }
        public string?  RejectionReason  { get; set; }
        public string   CustomerName     { get; set; }
        public string   CustomerNumber   { get; set; }
        public string?  ApprovedByStaff  { get; set; }
    }
}