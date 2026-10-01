using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingDbContext _context;

        public TransactionRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetByAccountAsync(int accountId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ReceiverAccount)
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetByCustomerAsync(int customerId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ReceiverAccount)
                .Where(t => t.Account.CustomerId == customerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByReferenceAsync(string referenceNumber)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.ReceiverAccount)
                .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);
        }

        public async Task<List<Transaction>> GetByDateRangeAsync(
            int accountId,
            DateTime from,
            DateTime to)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId &&
                            t.CreatedAt >= from &&
                            t.CreatedAt <= to)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            transaction.ReferenceNumber = NumberGenerator
                .GenerateTransactionReference();
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}