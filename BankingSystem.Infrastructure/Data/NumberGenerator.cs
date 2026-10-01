namespace BankingSystem.Infrastructure.Data
{
    public class NumberGenerator
    {
        public static string GenerateEmployeeNumber()
            => $"EMP{DateTime.Now:yyyyMMddHHmmss}";
        public static string GenerateCustomerNumber()
            => $"CUST{DateTime.Now:yyyyMMddHHmmss}";
        public static string GenerateAccountNumber()
            => $"ACC{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100,999)}";
        public static string GenerateLoanNumber()
            => $"LOAN{DateTime.Now:yyyyMMddHHmmss}";
        public static string GenerateTransactionReference()
            => $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000,9999)}";
    }
}