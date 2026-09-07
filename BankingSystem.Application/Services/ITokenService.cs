using BankingSystem.Domain.Models;

namespace BankingSystem.Application.Services
{
    public interface ITokenService
    {
        string GenerateStaffToken(Staff staff);
        string GenerateCustomerToken(Customer customer);
    }
}