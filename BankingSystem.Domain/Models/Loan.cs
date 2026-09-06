using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Models
{
    public class Loan
    {
        public int        Id              { get; set; }
        public string     LoanNumber      { get; set; }
        public LoanType   Type            { get; set; }
        public LoanStatus Status          { get; set; } = LoanStatus.Pending;
        public decimal    LoanAmount      { get; set; }
        public decimal?   ApprovedAmount  { get; set; }
        public decimal    InterestRate    { get; set; }
        public int        TermMonths      { get; set; }
        public decimal?   MonthlyPayment  { get; set; }
        public decimal    AmountRepaid    { get; set; } = 0;
        public string     Purpose         { get; set; }
        public DateTime   ApplicationDate { get; set; }
        public DateTime?  ApprovalDate    { get; set; }
        public DateTime?  DisbursementDate{ get; set; }
        public string?    RejectionReason { get; set; }

        // Foreign keys
        public int     CustomerId       { get; set; }
        public Customer Customer        { get; set; }

        public int?    ApprovedByStaffId { get; set; }
        public Staff?  ApprovedByStaff   { get; set; }

        // Which account loan is disbursed to
        public int?    AccountId { get; set; }
        public Account? Account  { get; set; }

        // Calculated — not stored
        public decimal RemainingAmount =>
            (ApprovedAmount ?? 0) - AmountRepaid;

        public bool IsFullyRepaid =>
            AmountRepaid >= (ApprovedAmount ?? 0);
    }
}