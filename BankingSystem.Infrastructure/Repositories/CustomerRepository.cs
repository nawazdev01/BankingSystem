using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class CustomerRepository :ICustomerRepository
    {
        private readonly BankingDbContext _context;
        public CustomerRepository(BankingDbContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c=>c.Id==id);
        }
        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _context.Customers.FirstOrDefaultAsync(c=>c.Username==username);
        }
        public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(c=>c.CustomerNumber==customerNumber);
        }
        public async Task<Customer> CreateAsync(Customer customer)
        {
           customer.CustomerNumber = NumberGenerator.GenerateCustomerNumber();
           customer.CreatedAt = DateTime.Now;
           _context.Add(customer);
           await _context.SaveChangesAsync();
           return customer; 
        }
        public async Task<Customer?> UpdateAsync(Customer customer)
        {
            var existing = await _context.Customers.FirstOrDefaultAsync(s=>s.Id==customer.Id);
            if(existing==null) return null;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.Email = customer.Email;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool?> DeactivateAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c=>c.Id==id);
            if(customer==null) return null;
            customer.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool?> VerifyKYCAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c=>c.Id==id);
            if(customer==null) return null;
            customer.IsKYCVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ExistsWithUsernameAsync(string username)
        {
            return await _context.Customers.AnyAsync(c=>c.Username==username);
        }
        public async Task<bool> ExistsWithEmailAsync(string email)
        {
            return await _context.Customers.AnyAsync(c=>c.Email==email);
        }
        public async Task<bool> ExistsWithNationalIdAsync(string nationalId)
        {
            return await _context.Customers.AnyAsync(c=>c.NationalId==nationalId);
        }
    }
}