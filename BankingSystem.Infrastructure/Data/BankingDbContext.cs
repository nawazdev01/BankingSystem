using BankingSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BankingSystem.Infrastructure.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) 
        : base(options)
        {
        }
        public DbSet<Staff>       Staff         { get; set; }
        public DbSet<Customer>    Customers     { get; set; }
        public DbSet<Account>     Accounts      { get; set; }
        public DbSet<Transaction> Transactions  { get; set; }
        public DbSet<Loan>        Loans         { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff> (entity =>
            {
                entity.HasIndex(s => s.Username).IsUnique();
                entity.HasIndex(s => s.Email).IsUnique();
                entity.HasIndex(s => s.EmployeeNumber).IsUnique();
                entity.Property(s => s.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(s => s.LastName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(s => s.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(s=>s.PasswordHash)
                      .IsRequired();
                entity.Property(s=>s.EmployeeNumber)
                      .IsRequired()
                      .HasMaxLength(20);
            });
            modelBuilder.Entity<Customer> (entity =>
            {
                entity.HasIndex(c=>c.Username).IsUnique();
                entity.HasIndex(c=>c.Email).IsUnique();
                entity.HasIndex(c=>c.CustomerNumber).IsUnique();
                entity.HasIndex(c=>c.NationalId).IsUnique();
                entity.Property(c=>c.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(c=>c.LastName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(c=>c.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(c=>c.PasswordHash)
                      .IsRequired();
                entity.Property(c=>c.CustomerNumber)
                      .IsRequired()
                      .HasMaxLength(20);
            });
              modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(a => a.AccountNumber).IsUnique();

                entity.Property(a => a.Balance)
                      .HasColumnType("decimal(18,2)");

                entity.Property(a => a.InterestRate)
                      .HasColumnType("decimal(5,2)");

                entity.Property(a => a.OverdraftLimit)
                      .HasColumnType("decimal(18,2)");

                entity.Property(a => a.DailyLimit)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(a => a.Customer)
                      .WithMany(c => c.Accounts)
                      .HasForeignKey(a => a.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
             // ── TRANSACTION ───────────────────────────────────
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(t => t.ReferenceNumber).IsUnique();

                entity.Property(t => t.Amount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(t => t.BalanceAfter)
                      .HasColumnType("decimal(18,2)");

                // Transaction belongs to one account
                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Transfer receiver — optional relationship
                entity.HasOne(t => t.ReceiverAccount)
                      .WithMany()
                      .HasForeignKey(t => t.ReceiverAccountId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });
             // ── LOAN ──────────────────────────────────────────
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasIndex(l => l.LoanNumber).IsUnique();

                entity.Property(l => l.LoanAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(l => l.ApprovedAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(l => l.InterestRate)
                      .HasColumnType("decimal(5,2)");

                entity.Property(l => l.MonthlyPayment)
                      .HasColumnType("decimal(18,2)");

                entity.Property(l => l.AmountRepaid)
                      .HasColumnType("decimal(18,2)");

                // Loan belongs to customer
                entity.HasOne(l => l.Customer)
                      .WithMany(c => c.Loans)
                      .HasForeignKey(l => l.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Loan approved by staff — optional
                entity.HasOne(l => l.ApprovedByStaff)
                      .WithMany(s => s.ApprovedLoans)
                      .HasForeignKey(l => l.ApprovedByStaffId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Loan disbursed to account — optional
                entity.HasOne(l => l.Account)
                      .WithMany(a => a.Loans)
                      .HasForeignKey(l => l.AccountId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Ignore calculated properties
                entity.Ignore(l => l.RemainingAmount);
                entity.Ignore(l => l.IsFullyRepaid);
            });
        }
    }
}