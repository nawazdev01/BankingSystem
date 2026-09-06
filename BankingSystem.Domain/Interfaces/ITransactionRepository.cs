using BankingSystem.Domain.Models;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetByAccountAsync(int accountId);
        Task<List<Transaction>> GetByCustomerAsync(int customerId);
        Task<Transaction?>      GetByReferenceAsync(string referenceNumber);
        Task<List<Transaction>> GetByDateRangeAsync(int accountId, DateTime from, DateTime to);
        Task<Transaction>       CreateAsync(Transaction transaction);
    }
}