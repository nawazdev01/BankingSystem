using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Services
{
    public interface IAccountService
    {
        Task<List<AccountResponse>> GetAllAsync();
        Task<AccountResponse?>      GetByIdAsync(int id);
        Task<List<AccountResponse>> GetByCustomerAsync(int customerId);
        Task<AccountResponse>       CreateAsync(CreateAccountRequest request);
        Task<bool?>                 FreezeAsync(int id);
        Task<bool?>                 UnfreezeAsync(int id);
        Task<bool?>                 CloseAsync(int id);
    }
}