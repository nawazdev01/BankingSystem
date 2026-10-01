using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;

namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository  _accountRepo;
        private readonly ICustomerRepository _customerRepo;

        public AccountService(
            IAccountRepository  accountRepo,
            ICustomerRepository customerRepo)
        {
            _accountRepo  = accountRepo;
            _customerRepo = customerRepo;
        }

        public async Task<List<AccountResponse>> GetAllAsync()
        {
            var accounts = await _accountRepo.GetAllAsync();
            return accounts.Select(a => MapToResponse(a)).ToList();
        }

        public async Task<AccountResponse?> GetByIdAsync(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            return account == null ? null : MapToResponse(account);
        }

        public async Task<List<AccountResponse>> GetByCustomerAsync(int customerId)
        {
            var accounts = await _accountRepo.GetByCustomerAsync(customerId);
            return accounts.Select(a => MapToResponse(a)).ToList();
        }

        public async Task<AccountResponse> CreateAsync(CreateAccountRequest request)
        {
            // Rule 1 — Customer must exist
            var customer = await _customerRepo.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException(
                    $"Customer with id {request.CustomerId} not found");

            // Rule 2 — Customer must be active
            if (!customer.IsActive)
                throw new InvalidOperationException(
                    "Cannot create account for inactive customer");

            // Rule 3 — Customer must be KYC verified
            if (!customer.IsKYCVerified)
                throw new InvalidOperationException(
                    "Customer must be KYC verified before opening an account");

            // Rule 4 — Fixed deposit must have maturity date
            if (request.Type == AccountType.FixedDeposit &&
                request.MaturityDate == null)
                throw new InvalidOperationException(
                    "Fixed deposit account requires a maturity date");

            // Rule 5 — Initial deposit must be positive
            if (request.InitialDeposit <= 0)
                throw new InvalidOperationException(
                    "Initial deposit must be greater than zero");

            var account = new Account
            {
                CustomerId    = request.CustomerId,
                Type          = request.Type,
                InterestRate  = request.InterestRate,
                OverdraftLimit= request.OverdraftLimit,
                MaturityDate  = request.MaturityDate,
                Status        = AccountStatus.Active
            };

            // Use domain method to set initial balance
            account.Deposit(request.InitialDeposit);

            var created = await _accountRepo.CreateAsync(account);
            return MapToResponse(created);
        }

        public async Task<bool?> FreezeAsync(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null) return null;

            if (account.Status == AccountStatus.Frozen)
                throw new InvalidOperationException(
                    "Account is already frozen");

            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException(
                    "Cannot freeze a closed account");

            account.Status = AccountStatus.Frozen;
            await _accountRepo.UpdateAsync(account);
            return true;
        }

        public async Task<bool?> UnfreezeAsync(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null) return null;

            if (account.Status != AccountStatus.Frozen)
                throw new InvalidOperationException(
                    "Account is not frozen");

            account.Status = AccountStatus.Active;
            await _accountRepo.UpdateAsync(account);
            return true;
        }

        public async Task<bool?> CloseAsync(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null) return null;

            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException(
                    "Account is already closed");

            if (account.Balance > 0)
                throw new InvalidOperationException(
                    $"Cannot close account with remaining balance of {account.Balance:C}. Please withdraw all funds first.");

            account.Status = AccountStatus.Closed;
            await _accountRepo.UpdateAsync(account);
            return true;
        }

        private AccountResponse MapToResponse(Account account)
        {
            return new AccountResponse
            {
                Id             = account.Id,
                AccountNumber  = account.AccountNumber,
                Type           = account.Type.ToString(),
                Status         = account.Status.ToString(),
                Balance        = account.Balance,
                OpenedDate     = account.OpenedDate,
                DailyLimit     = account.DailyLimit,
                InterestRate   = account.InterestRate,
                OverdraftLimit = account.OverdraftLimit,
                MaturityDate   = account.MaturityDate,
                CustomerName   = account.Customer != null
                    ? $"{account.Customer.FirstName} {account.Customer.LastName}"
                    : "Unknown",
                CustomerNumber = account.Customer?.CustomerNumber ?? "Unknown"
            };
        }
    }
}