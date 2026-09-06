using BankingSystem.Domain.Models;

namespace BankingSystem.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?>      GetByIdAsync(int id);
        Task<Customer?>      GetByUsernameAsync(string username);
        Task<Customer?>      GetByCustomerNumberAsync(string customerNumber);
        Task<Customer>       CreateAsync(Customer customer);
        Task<Customer?>      UpdateAsync(Customer customer);
        Task<bool?>          DeactivateAsync(int id);
        Task<bool>           ExistsWithUsernameAsync(string username);
        Task<bool>           ExistsWithEmailAsync(string email);
        Task<bool>           ExistsWithNationalIdAsync(string nationalId);
    }
}