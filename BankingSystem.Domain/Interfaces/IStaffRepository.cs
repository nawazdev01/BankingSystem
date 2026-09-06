using BankingSystem.Domain.Models;

namespace BankingSystem.Domain.Interfaces
{
    public interface IStaffRepository
    {
        Task<List<Staff>>  GetAllAsync();
        Task<Staff?>       GetByIdAsync(int id);
        Task<Staff?>       GetByUsernameAsync(string username);
        Task<Staff>        CreateAsync(Staff staff);
        Task<Staff?>       UpdateAsync(Staff staff);
        Task<bool?>        DeactivateAsync(int id);
        Task<bool>         ExistsWithUsernameAsync(string username);
        Task<bool>         ExistsWithEmployeeNumberAsync(string employeeNumber);
        Task<bool>         ExistsWithEmailAsync(string email);
    }
}