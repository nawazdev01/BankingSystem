using BankingSystem.Domain.Models;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?>      GetByIdAsync(int id);
        Task<Account?>      GetByAccountNumberAsync(string accountNumber);
        Task<List<Account>> GetByCustomerAsync(int customerId);
        Task<List<Account>> GetByTypeAsync(AccountType type);
        Task<Account>       CreateAsync(Account account);
        Task<Account?>      UpdateAsync(Account account);
        Task<bool>          ExistsWithAccountNumberAsync(string accountNumber);
    }
}