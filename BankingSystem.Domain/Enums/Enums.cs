namespace BankingSystem.Domain.Enums
{
    public enum AccountType
    {
        Savings,
        Current,
        FixedDeposit
    }

    public enum AccountStatus
    {
        Active,
        Frozen,
        Closed
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Interest,
        LoanDisbursement,
        LoanRepayment
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Reversed
    }

    public enum LoanType
    {
        Personal,
        Home,
        Car,
        Business
    }

    public enum LoanStatus
    {
        Pending,
        Approved,
        Rejected,
        Disbursed,
        Repaid,
        Defaulted
    }

    public enum StaffRole
    {
        Admin,
        Manager,
        Teller,
        LoanOfficer
    }
}