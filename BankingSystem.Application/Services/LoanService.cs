using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;

namespace BankingSystem.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository        _loanRepo;
        private readonly ICustomerRepository    _customerRepo;
        private readonly IAccountRepository     _accountRepo;
        private readonly ITransactionRepository _transactionRepo;

        public LoanService(
            ILoanRepository        loanRepo,
            ICustomerRepository    customerRepo,
            IAccountRepository     accountRepo,
            ITransactionRepository transactionRepo)
        {
            _loanRepo        = loanRepo;
            _customerRepo    = customerRepo;
            _accountRepo     = accountRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<List<LoanResponse>> GetAllAsync()
        {
            var loans = await _loanRepo.GetAllAsync();
            return loans.Select(l => MapToResponse(l)).ToList();
        }

        public async Task<LoanResponse?> GetByIdAsync(int id)
        {
            var loan = await _loanRepo.GetByIdAsync(id);
            return loan == null ? null : MapToResponse(loan);
        }

        public async Task<List<LoanResponse>> GetByCustomerAsync(int customerId)
        {
            var loans = await _loanRepo.GetByCustomerAsync(customerId);
            return loans.Select(l => MapToResponse(l)).ToList();
        }

        public async Task<List<LoanResponse>> GetByStatusAsync(LoanStatus status)
        {
            var loans = await _loanRepo.GetByStatusAsync(status);
            return loans.Select(l => MapToResponse(l)).ToList();
        }

        public async Task<LoanResponse> ApplyAsync(ApplyLoanRequest request)
        {
            // Rule 1 — Customer must exist
            var customer = await _customerRepo.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException(
                    $"Customer with id {request.CustomerId} not found");

            // Rule 2 — Customer must be active
            if (!customer.IsActive)
                throw new InvalidOperationException(
                    "Inactive customer cannot apply for a loan");

            // Rule 3 — Customer must be KYC verified
            if (!customer.IsKYCVerified)
                throw new InvalidOperationException(
                    "Customer must be KYC verified to apply for a loan");

            // Rule 4 — Account must exist and belong to customer
            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new KeyNotFoundException(
                    $"Account with id {request.AccountId} not found");

            if (account.CustomerId != request.CustomerId)
                throw new InvalidOperationException(
                    "Account does not belong to this customer");

            // Rule 5 — Loan amount must be positive
            if (request.LoanAmount <= 0)
                throw new InvalidOperationException(
                    "Loan amount must be greater than zero");

            // Rule 6 — Term must be valid
            if (request.TermMonths <= 0)
                throw new InvalidOperationException(
                    "Loan term must be greater than zero months");

            var loan = new Loan
            {
                CustomerId  = request.CustomerId,
                AccountId   = request.AccountId,
                Type        = request.Type,
                LoanAmount  = request.LoanAmount,
                TermMonths  = request.TermMonths,
                Purpose     = request.Purpose,
                Status      = LoanStatus.Pending,
                InterestRate= 0  // set by staff on approval
            };

            var created = await _loanRepo.CreateAsync(loan);
            return MapToResponse(created);
        }

        public async Task<LoanResponse?> ApproveAsync(
            int id,
            int staffId,
            ApproveLoanRequest request)
        {
            var loan = await _loanRepo.GetByIdAsync(id);
            if (loan == null) return null;

            if (loan.Status != LoanStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending loans can be approved");

            // Calculate monthly payment using standard formula
            // M = P[r(1+r)^n]/[(1+r)^n-1]
            // P = principal, r = monthly rate, n = term months
            decimal monthlyRate    = request.InterestRate / 100 / 12;
            decimal principal      = request.ApprovedAmount;
            int     n              = loan.TermMonths;
            decimal monthlyPayment = 0;

            if (monthlyRate > 0)
            {
                double factor = Math.Pow((double)(1 + monthlyRate), n);
                monthlyPayment = principal *
                    (monthlyRate * (decimal)factor) /
                    ((decimal)factor - 1);
            }
            else
            {
                // Zero interest — equal installments
                monthlyPayment = principal / n;
            }

            loan.Status            = LoanStatus.Approved;
            loan.ApprovedAmount    = request.ApprovedAmount;
            loan.InterestRate      = request.InterestRate;
            loan.MonthlyPayment    = Math.Round(monthlyPayment, 2);
            loan.ApprovalDate      = DateTime.UtcNow;
            loan.ApprovedByStaffId = staffId;

            var updated = await _loanRepo.UpdateAsync(loan);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<LoanResponse?> RejectAsync(
            int id,
            int staffId,
            RejectLoanRequest request)
        {
            var loan = await _loanRepo.GetByIdAsync(id);
            if (loan == null) return null;

            if (loan.Status != LoanStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending loans can be rejected");

            loan.Status            = LoanStatus.Rejected;
            loan.RejectionReason   = request.RejectionReason;
            loan.ApprovalDate      = DateTime.UtcNow;
            loan.ApprovedByStaffId = staffId;

            var updated = await _loanRepo.UpdateAsync(loan);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<LoanResponse?> DisburseAsync(int id)
        {
            var loan = await _loanRepo.GetByIdAsync(id);
            if (loan == null) return null;

            if (loan.Status != LoanStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved loans can be disbursed");

            // Get the account to deposit into
            var account = await _accountRepo.GetByIdAsync(loan.AccountId!.Value);
            if (account == null)
                throw new KeyNotFoundException("Loan account not found");

            // Deposit loan amount into account
            account.Deposit(loan.ApprovedAmount!.Value);
            await _accountRepo.UpdateAsync(account);

            // Create transaction record
            var transaction = new Transaction
            {
                AccountId    = account.Id,
                Amount       = loan.ApprovedAmount!.Value,
                Type         = TransactionType.LoanDisbursement,
                Status       = TransactionStatus.Completed,
                Description  = $"Loan disbursement - {loan.LoanNumber}",
                BalanceAfter = account.Balance
            };
            await _transactionRepo.CreateAsync(transaction);

            // Update loan status
            loan.Status           = LoanStatus.Disbursed;
            loan.DisbursementDate = DateTime.UtcNow;

            var updated = await _loanRepo.UpdateAsync(loan);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<TransactionResponse> RepayAsync(
            LoanRepaymentRequest request)
        {
            var loan = await _loanRepo.GetByIdAsync(request.LoanId);
            if (loan == null)
                throw new KeyNotFoundException(
                    $"Loan with id {request.LoanId} not found");

            if (loan.Status != LoanStatus.Disbursed)
                throw new InvalidOperationException(
                    "Can only repay disbursed loans");

            if (loan.IsFullyRepaid)
                throw new InvalidOperationException(
                    "Loan is already fully repaid");

            if (request.Amount <= 0)
                throw new InvalidOperationException(
                    "Repayment amount must be positive");

            if (request.Amount > loan.RemainingAmount)
                throw new InvalidOperationException(
                    $"Repayment amount exceeds remaining balance of {loan.RemainingAmount:C}");

            // Deduct from account
            var account = await _accountRepo
                .GetByIdAsync(loan.AccountId!.Value);
            if (account == null)
                throw new KeyNotFoundException("Loan account not found");

            account.Withdraw(request.Amount);
            await _accountRepo.UpdateAsync(account);

            // Update loan repayment
            loan.AmountRepaid += request.Amount;

            // If fully repaid — mark as repaid
            if (loan.IsFullyRepaid)
                loan.Status = LoanStatus.Repaid;

            await _loanRepo.UpdateAsync(loan);

            // Create transaction record
            var transaction = new Transaction
            {
                AccountId    = account.Id,
                Amount       = request.Amount,
                Type         = TransactionType.LoanRepayment,
                Status       = TransactionStatus.Completed,
                Description  = $"Loan repayment - {loan.LoanNumber}",
                BalanceAfter = account.Balance
            };

            var created = await _transactionRepo.CreateAsync(transaction);
            return new TransactionResponse
            {
                Id              = created.Id,
                ReferenceNumber = created.ReferenceNumber,
                Amount          = created.Amount,
                Type            = created.Type.ToString(),
                Status          = created.Status.ToString(),
                Description     = created.Description,
                BalanceAfter    = created.BalanceAfter,
                CreatedAt       = created.CreatedAt,
                AccountNumber   = account.AccountNumber
            };
        }

        private LoanResponse MapToResponse(Loan loan)
        {
            return new LoanResponse
            {
                Id              = loan.Id,
                LoanNumber      = loan.LoanNumber,
                Type            = loan.Type.ToString(),
                Status          = loan.Status.ToString(),
                LoanAmount      = loan.LoanAmount,
                ApprovedAmount  = loan.ApprovedAmount,
                InterestRate    = loan.InterestRate,
                TermMonths      = loan.TermMonths,
                MonthlyPayment  = loan.MonthlyPayment,
                AmountRepaid    = loan.AmountRepaid,
                RemainingAmount = loan.RemainingAmount,
                Purpose         = loan.Purpose,
                ApplicationDate = loan.ApplicationDate,
                ApprovalDate    = loan.ApprovalDate,
                RejectionReason = loan.RejectionReason,
                CustomerName    = loan.Customer != null
                    ? $"{loan.Customer.FirstName} {loan.Customer.LastName}"
                    : "Unknown",
                CustomerNumber  = loan.Customer?.CustomerNumber ?? "Unknown",
                ApprovedByStaff = loan.ApprovedByStaff != null
                    ? $"{loan.ApprovedByStaff.FirstName} {loan.ApprovedByStaff.LastName}"
                    : null
            };
        }
    }
}