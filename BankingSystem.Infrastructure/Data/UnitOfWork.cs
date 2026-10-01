using BankingSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankingSystem.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;
        private IDbContextTransaction?   _transaction;

        public UnitOfWork(BankingDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database
                .BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction!.RollbackAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}