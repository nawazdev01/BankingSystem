using BankingSystem.Application.DTOs;

namespace BankingSystem.Application.Services
{
    public interface IStaffService
    {
        Task<List<StaffResponse>> GetAllAsync();
        Task<StaffResponse?>      GetByIdAsync(int id);
        Task<StaffResponse>       RegisterAsync(RegisterStaffRequest request);
        Task<AuthResponse?>       LoginAsync(StaffLoginRequest request);
        Task<StaffResponse?>      UpdateAsync(int id, RegisterStaffRequest request);
        Task<bool?>               DeactivateAsync(int id);
    }
}