using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Models
{
    public class Staff
    {
        public int      Id             { get; set; }
        public string   FirstName      { get; set; }
        public string   LastName       { get; set; }
        public string   Email          { get; set; }
        public string   PhoneNumber    { get; set; }
        public string   EmployeeNumber { get; set; }
        public string   Username       { get; set; }
        public string   PasswordHash   { get; set; }
        public StaffRole Role          { get; set; }
        public DateTime JoinedDate     { get; set; }
        public bool     IsActive       { get; set; } = true;

        // Staff approves loans
        public List<Loan> ApprovedLoans { get; set; } = new();
    }
}