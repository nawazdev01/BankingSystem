using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly BankingDbContext _context;

        public LoanRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Loan>> GetAllAsync()
        {
            return await _context.Loans
                .Include(l => l.Customer)
                .Include(l => l.ApprovedByStaff)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Loan?> GetByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Customer)
                .Include(l => l.ApprovedByStaff)
                .Include(l => l.Account)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Loan>> GetByCustomerAsync(int customerId)
        {
            return await _context.Loans
                .Include(l => l.ApprovedByStaff)
                .Where(l => l.CustomerId == customerId)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetByStatusAsync(LoanStatus status)
        {
            return await _context.Loans
                .Include(l => l.Customer)
                .Include(l => l.ApprovedByStaff)
                .Where(l => l.Status == status)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Loan> CreateAsync(Loan loan)
        {
            loan.LoanNumber      = NumberGenerator.GenerateLoanNumber();
            loan.ApplicationDate = DateTime.UtcNow;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<Loan?> UpdateAsync(Loan loan)
        {
            var existing = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loan.Id);

            if (existing == null) return null;

            existing.Status           = loan.Status;
            existing.ApprovedAmount   = loan.ApprovedAmount;
            existing.InterestRate     = loan.InterestRate;
            existing.MonthlyPayment   = loan.MonthlyPayment;
            existing.ApprovalDate     = loan.ApprovalDate;
            existing.DisbursementDate = loan.DisbursementDate;
            existing.RejectionReason  = loan.RejectionReason;
            existing.ApprovedByStaffId= loan.ApprovedByStaffId;
            existing.AccountId        = loan.AccountId;
            existing.AmountRepaid     = loan.AmountRepaid;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}