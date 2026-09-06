using BankingSystem.Domain.Models;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Interfaces
{
    public interface ILoanRepository
    {
        Task<List<Loan>> GetAllAsync();
        Task<Loan?>      GetByIdAsync(int id);
        Task<List<Loan>> GetByCustomerAsync(int customerId);
        Task<List<Loan>> GetByStatusAsync(LoanStatus status);
        Task<Loan>       CreateAsync(Loan loan);
        Task<Loan?>      UpdateAsync(Loan loan);
    }
}