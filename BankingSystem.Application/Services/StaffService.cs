using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repo;
        private readonly ITokenService    _tokenService;

        public StaffService(
            IStaffRepository repo,
            ITokenService    tokenService)
        {
            _repo         = repo;
            _tokenService = tokenService;
        }

        public async Task<List<StaffResponse>> GetAllAsync()
        {
            var staff = await _repo.GetAllAsync();
            return staff.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<StaffResponse?> GetByIdAsync(int id)
        {
            var staff = await _repo.GetByIdAsync(id);
            return staff == null ? null : MapToResponse(staff);
        }

        public async Task<StaffResponse> RegisterAsync(RegisterStaffRequest request)
        {
            // Rule 1 — username must be unique
            if (await _repo.ExistsWithUsernameAsync(request.Username))
                throw new InvalidOperationException(
                    $"Username '{request.Username}' is already taken");

            // Rule 2 — email must be unique
            if (await _repo.ExistsWithEmailAsync(request.Email))
                throw new InvalidOperationException(
                    $"Email '{request.Email}' is already registered");

            var staff = new Staff
            {
                FirstName    = request.FirstName,
                LastName     = request.LastName,
                Email        = request.Email,
                PhoneNumber  = request.PhoneNumber,
                Username     = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role         = request.Role,
                IsActive     = true
            };

            var created = await _repo.CreateAsync(staff);
            return MapToResponse(created);
        }

        public async Task<AuthResponse?> LoginAsync(StaffLoginRequest request)
        {
            // Find staff by username
            var staff = await _repo.GetByUsernameAsync(request.Username);
            if (staff == null) return null;

            // Check if active
            if (!staff.IsActive)
                throw new InvalidOperationException(
                    "This account has been deactivated");

            // Verify password
            bool passwordCorrect = BCrypt.Net.BCrypt.Verify(
                request.Password, staff.PasswordHash);

            if (!passwordCorrect) return null;

            // Generate token
            var token = _tokenService.GenerateStaffToken(staff);

            return new AuthResponse
            {
                Token    = token,
                Username = staff.Username,
                Role     = staff.Role.ToString(),
                Id       = staff.Id
            };
        }

        public async Task<StaffResponse?> UpdateAsync(
            int id,
            RegisterStaffRequest request)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.FirstName   = request.FirstName;
            existing.LastName    = request.LastName;
            existing.Email       = request.Email;
            existing.PhoneNumber = request.PhoneNumber;
            existing.Role        = request.Role;

            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool?> DeactivateAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (!existing.IsActive)
                throw new InvalidOperationException(
                    "Staff member is already deactivated");

            return await _repo.DeactivateAsync(id);
        }

        // ── Private mapping method ────────────────────────────
        private StaffResponse MapToResponse(Staff staff)
        {
            return new StaffResponse
            {
                Id             = staff.Id,
                FirstName      = staff.FirstName,
                LastName       = staff.LastName,
                FullName       = $"{staff.FirstName} {staff.LastName}",
                Email          = staff.Email,
                PhoneNumber    = staff.PhoneNumber,
                EmployeeNumber = staff.EmployeeNumber,
                Role           = staff.Role.ToString(),
                JoinedDate     = staff.JoinedDate,
                IsActive       = staff.IsActive
            };
        }
    }
}