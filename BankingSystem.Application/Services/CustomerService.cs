using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;

namespace BankingSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly ITokenService       _tokenService;

        public CustomerService(
            ICustomerRepository repo,
            ITokenService       tokenService)
        {
            _repo         = repo;
            _tokenService = tokenService;
        }

        public async Task<List<CustomerResponse>> GetAllAsync()
        {
            var customers = await _repo.GetAllAsync();
            return customers.Select(c => MapToResponse(c)).ToList();
        }

        public async Task<CustomerResponse?> GetByIdAsync(int id)
        {
            var customer = await _repo.GetByIdAsync(id);
            return customer == null ? null : MapToResponse(customer);
        }

        public async Task<CustomerResponse> RegisterAsync(
            RegisterCustomerRequest request)
        {
            // Rule 1 — username must be unique
            if (await _repo.ExistsWithUsernameAsync(request.Username))
                throw new InvalidOperationException(
                    $"Username '{request.Username}' is already taken");

            // Rule 2 — email must be unique
            if (await _repo.ExistsWithEmailAsync(request.Email))
                throw new InvalidOperationException(
                    $"Email '{request.Email}' is already registered");

            // Rule 3 — national ID must be unique
            if (await _repo.ExistsWithNationalIdAsync(request.NationalId))
                throw new InvalidOperationException(
                    $"National ID '{request.NationalId}' is already registered");

            var customer = new Customer
            {
                FirstName    = request.FirstName,
                LastName     = request.LastName,
                Email        = request.Email,
                PhoneNumber  = request.PhoneNumber,
                Address      = request.Address,
                DateOfBirth  = request.DateOfBirth,
                Username     = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                NationalId   = request.NationalId,
                IsKYCVerified= false,
                IsActive     = true
            };

            var created = await _repo.CreateAsync(customer);
            return MapToResponse(created);
        }

        public async Task<AuthResponse?> LoginAsync(CustomerLoginRequest request)
        {
            var customer = await _repo.GetByUsernameAsync(request.Username);
            if (customer == null) return null;

            if (!customer.IsActive)
                throw new InvalidOperationException(
                    "This account has been deactivated");

            bool passwordCorrect = BCrypt.Net.BCrypt.Verify(
                request.Password, customer.PasswordHash);

            if (!passwordCorrect) return null;

            var token = _tokenService.GenerateCustomerToken(customer);

            return new AuthResponse
            {
                Token    = token,
                Username = customer.Username,
                Role     = "Customer",
                Id       = customer.Id
            };
        }

        public async Task<CustomerResponse?> UpdateAsync(
            int id,
            UpdateCustomerRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (!existing.IsActive)
                throw new InvalidOperationException(
                    "Cannot update deactivated customer");

            existing.PhoneNumber = request.PhoneNumber;
            existing.Address     = request.Address;
            existing.Email       = request.Email;

            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool?> DeactivateAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (!existing.IsActive)
                throw new InvalidOperationException(
                    "Customer is already deactivated");

            return await _repo.DeactivateAsync(id);
        }

        public async Task<bool?> VerifyKYCAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (existing.IsKYCVerified)
                throw new InvalidOperationException(
                    "Customer KYC is already verified");

            return await _repo.VerifyKYCAsync(id);
        }

        private CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id             = customer.Id,
                FirstName      = customer.FirstName,
                LastName       = customer.LastName,
                FullName       = $"{customer.FirstName} {customer.LastName}",
                Email          = customer.Email,
                PhoneNumber    = customer.PhoneNumber,
                Address        = customer.Address,
                DateOfBirth    = customer.DateOfBirth,
                Age            = DateTime.Today.Year - customer.DateOfBirth.Year,
                CustomerNumber = customer.CustomerNumber,
                IsKYCVerified  = customer.IsKYCVerified,
                IsActive       = customer.IsActive,
                CreatedAt      = customer.CreatedAt
            };
        }
    }
}