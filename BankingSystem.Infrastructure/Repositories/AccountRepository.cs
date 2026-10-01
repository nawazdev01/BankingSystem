using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<List<Account>> GetByCustomerAsync(int customerId)
        {
            return await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<List<Account>> GetByTypeAsync(AccountType type)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Where(a => a.Type == type)
                .ToListAsync();
        }

        public async Task<Account> CreateAsync(Account account)
        {
            account.AccountNumber = NumberGenerator.GenerateAccountNumber();
            account.OpenedDate    = DateTime.UtcNow;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> UpdateAsync(Account account)
        {
            var existing = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            if (existing == null) return null;

            existing.Status        = account.Status;
            existing.DailyLimit    = account.DailyLimit;
            existing.OverdraftLimit= account.OverdraftLimit;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ExistsWithAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .AnyAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}