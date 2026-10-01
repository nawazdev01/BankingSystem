using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;

namespace BankingSystem.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository     _accountRepo;
        private readonly IUnitOfWork            _unitOfWork;

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository     accountRepo,
            IUnitOfWork            unitOfWork)
        {
            _transactionRepo = transactionRepo;
            _accountRepo     = accountRepo;
            _unitOfWork      = unitOfWork;
        }

        public async Task<TransactionResponse> DepositAsync(DepositRequest request)
        {
            // Find account
            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new KeyNotFoundException(
                    $"Account with id {request.AccountId} not found");

            // Validate
            if (request.Amount <= 0)
                throw new InvalidOperationException(
                    "Deposit amount must be positive");

            // Use domain method — has validation built in
            account.Deposit(request.Amount);

            // Save updated balance
            await _accountRepo.UpdateAsync(account);

            // Create transaction record
            var transaction = new Transaction
            {
                AccountId    = account.Id,
                Amount       = request.Amount,
                Type         = TransactionType.Deposit,
                Status       = TransactionStatus.Completed,
                Description  = request.Description ?? "Deposit",
                BalanceAfter = account.Balance
            };

            var created = await _transactionRepo.CreateAsync(transaction);
            return MapToResponse(created);
        }

        public async Task<TransactionResponse> WithdrawAsync(WithdrawRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new KeyNotFoundException(
                    $"Account with id {request.AccountId} not found");

            if (request.Amount <= 0)
                throw new InvalidOperationException(
                    "Withdrawal amount must be positive");

            // Check daily limit
            if (request.Amount > account.DailyLimit)
                throw new InvalidOperationException(
                    $"Amount exceeds daily limit of {account.DailyLimit:C}");

            // Use domain method — throws if insufficient funds
            account.Withdraw(request.Amount);

            await _accountRepo.UpdateAsync(account);

            var transaction = new Transaction
            {
                AccountId    = account.Id,
                Amount       = request.Amount,
                Type         = TransactionType.Withdrawal,
                Status       = TransactionStatus.Completed,
                Description  = request.Description ?? "Withdrawal",
                BalanceAfter = account.Balance
            };

            var created = await _transactionRepo.CreateAsync(transaction);
            return MapToResponse(created);
        }

        public async Task<TransactionResponse> TransferAsync(TransferRequest request)
        {
            // Find both accounts
            var senderAccount = await _accountRepo
                .GetByIdAsync(request.SenderAccountId);
            if (senderAccount == null)
                throw new KeyNotFoundException(
                    $"Sender account with id {request.SenderAccountId} not found");

            var receiverAccount = await _accountRepo
                .GetByIdAsync(request.ReceiverAccountId);
            if (receiverAccount == null)
                throw new KeyNotFoundException(
                    $"Receiver account with id {request.ReceiverAccountId} not found");

            // Cannot transfer to same account
            if (request.SenderAccountId == request.ReceiverAccountId)
                throw new InvalidOperationException(
                    "Cannot transfer to the same account");

            if (request.Amount <= 0)
                throw new InvalidOperationException(
                    "Transfer amount must be positive");

            if (request.Amount > senderAccount.DailyLimit)
                throw new InvalidOperationException(
                    $"Amount exceeds daily limit of {senderAccount.DailyLimit:C}");

            // ── Begin atomic transaction ──────────────────────
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Deduct from sender — domain method validates funds
                senderAccount.Withdraw(request.Amount);
                await _accountRepo.UpdateAsync(senderAccount);

                // Add to receiver
                receiverAccount.Deposit(request.Amount);
                await _accountRepo.UpdateAsync(receiverAccount);

                // Create transaction record
                var transaction = new Transaction
                {
                    AccountId         = senderAccount.Id,
                    ReceiverAccountId = receiverAccount.Id,
                    Amount            = request.Amount,
                    Type              = TransactionType.Transfer,
                    Status            = TransactionStatus.Completed,
                    Description       = request.Description ?? "Transfer",
                    BalanceAfter      = senderAccount.Balance
                };

                await _transactionRepo.CreateAsync(transaction);

                // ── Commit — all three operations succeed together
                await _unitOfWork.CommitAsync();

                return MapToResponse(transaction);
            }
            catch
            {
                // ── Rollback — undo everything if anything failed
                await _unitOfWork.RollbackAsync();
                throw;  // rethrow so controller can handle it
            }
        }
         

        public async Task<List<TransactionResponse>> GetByAccountAsync(int accountId)
        {
            var transactions = await _transactionRepo.GetByAccountAsync(accountId);
            return transactions.Select(t => MapToResponse(t)).ToList();
        }

        public async Task<List<TransactionResponse>> GetByCustomerAsync(int customerId)
        {
            var transactions = await _transactionRepo.GetByCustomerAsync(customerId);
            return transactions.Select(t => MapToResponse(t)).ToList();
        }

        public async Task<TransactionResponse?> GetByReferenceAsync(string reference)
        {
            var transaction = await _transactionRepo.GetByReferenceAsync(reference);
            return transaction == null ? null : MapToResponse(transaction);
        }

        public async Task<List<TransactionResponse>> GetByDateRangeAsync(
            int accountId, DateTime from, DateTime to)
        {
            var transactions = await _transactionRepo
                .GetByDateRangeAsync(accountId, from, to);
            return transactions.Select(t => MapToResponse(t)).ToList();
        }

        private TransactionResponse MapToResponse(Transaction transaction)
        {
            return new TransactionResponse
            {
                Id                    = transaction.Id,
                ReferenceNumber       = transaction.ReferenceNumber,
                Amount                = transaction.Amount,
                Type                  = transaction.Type.ToString(),
                Status                = transaction.Status.ToString(),
                Description           = transaction.Description,
                BalanceAfter          = transaction.BalanceAfter,
                FailureReason         = transaction.FailureReason,
                CreatedAt             = transaction.CreatedAt,
                AccountNumber         = transaction.Account?.AccountNumber ?? "Unknown",
                ReceiverAccountNumber = transaction.ReceiverAccount?.AccountNumber
            };
        }
    }
}