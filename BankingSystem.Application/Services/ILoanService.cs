using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Services
{
    public interface ILoanService
    {
        Task<List<LoanResponse>> GetAllAsync();
        Task<LoanResponse?>      GetByIdAsync(int id);
        Task<List<LoanResponse>> GetByCustomerAsync(int customerId);
        Task<List<LoanResponse>> GetByStatusAsync(LoanStatus status);
        Task<LoanResponse>       ApplyAsync(ApplyLoanRequest request);
        Task<LoanResponse?>      ApproveAsync(int id, int staffId, ApproveLoanRequest request);
        Task<LoanResponse?>      RejectAsync(int id, int staffId, RejectLoanRequest request);
        Task<LoanResponse?>      DisburseAsync(int id);
        Task<TransactionResponse> RepayAsync(LoanRepaymentRequest request);
    }
}