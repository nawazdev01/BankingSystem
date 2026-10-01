using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BankingSystem.Infrastructure.Repositories
{
    public class StaffRepository :IStaffRepository
    {
        private readonly BankingDbContext _context;
        public StaffRepository(BankingDbContext context)
        {
            _context = context;
        }
        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .OrderBy(s=>s.LastName)
                .ToListAsync();
        }
        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _context.Staff.FirstOrDefaultAsync(s=>s.Id==id);
        }
        public async Task<Staff?> GetByUsernameAsync(string username)
        {
            return await _context.Staff.FirstOrDefaultAsync(s=>s.Username.ToLower()==username);
        }
        public async Task<Staff> CreateAsync(Staff staff)
        {
            staff.EmployeeNumber = NumberGenerator.GenerateEmployeeNumber();
            staff.JoinedDate = DateTime.Now;
            _context.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }
        public async Task<Staff?> UpdateAsync(Staff staff)
        {
            var existing = await _context.Staff.FirstOrDefaultAsync(s=>s.Id==staff.Id);
            if(existing==null)
                return null;
            existing.FirstName = staff.FirstName;
            existing.LastName = staff.LastName;
            existing.Email = staff.Email;
            existing.PhoneNumber = staff.PhoneNumber;
            existing.EmployeeNumber = staff.EmployeeNumber;
            existing.Username = staff.Username;
            existing.PasswordHash = staff.PasswordHash;
            existing.Role = staff.Role;
            existing.JoinedDate = staff.JoinedDate;
            existing.IsActive = staff.IsActive;
            _context.Add(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool?> DeactivateAsync(int id)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s=>s.Id==id);
            if(staff==null) return null;
            staff.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ExistsWithUsernameAsync(string username)
        {
            return await _context.Staff.AnyAsync(s=>s.Username==username);
        }
        public async Task<bool> ExistsWithEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Staff.AnyAsync(s=>s.EmployeeNumber==employeeNumber);
        }
        public async Task<bool> ExistsWithEmailAsync(string email)
        {
            return await _context.Staff.AnyAsync(s=>s.Email==email);
        }

    }
}