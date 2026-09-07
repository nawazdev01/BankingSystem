using BankingSystem.Application.DTOs;

namespace BankingSystem.Application.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponse>       DepositAsync(DepositRequest request);
        Task<TransactionResponse>       WithdrawAsync(WithdrawRequest request);
        Task<TransactionResponse>       TransferAsync(TransferRequest request);
        Task<List<TransactionResponse>> GetByAccountAsync(int accountId);
        Task<List<TransactionResponse>> GetByCustomerAsync(int customerId);
        Task<TransactionResponse?>      GetByReferenceAsync(string reference);
        Task<List<TransactionResponse>> GetByDateRangeAsync(int accountId, DateTime from, DateTime to);
    }
}