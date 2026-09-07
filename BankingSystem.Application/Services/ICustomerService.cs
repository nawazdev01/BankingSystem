using BankingSystem.Application.DTOs;

namespace BankingSystem.Application.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerResponse>> GetAllAsync();
        Task<CustomerResponse?>      GetByIdAsync(int id);
        Task<CustomerResponse>       RegisterAsync(RegisterCustomerRequest request);
        Task<AuthResponse?>          LoginAsync(CustomerLoginRequest request);
        Task<CustomerResponse?>      UpdateAsync(int id, UpdateCustomerRequest request);
        Task<bool?>                  DeactivateAsync(int id);
        Task<bool?>                  VerifyKYCAsync(int id);
    }
}