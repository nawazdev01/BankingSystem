using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTOs
{
    // Request — register new staff
    public class RegisterStaffRequest
    {
        public string    FirstName   { get; set; }
        public string    LastName    { get; set; }
        public string    Email       { get; set; }
        public string    PhoneNumber { get; set; }
        public string    Username    { get; set; }
        public string    Password    { get; set; }
        public StaffRole Role        { get; set; }
    }

    // Request — staff login
    public class StaffLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // Response — what we send back
    public class StaffResponse
    {
        public int      Id             { get; set; }
        public string   FirstName      { get; set; }
        public string   LastName       { get; set; }
        public string   FullName       { get; set; }
        public string   Email          { get; set; }
        public string   PhoneNumber    { get; set; }
        public string   EmployeeNumber { get; set; }
        public string   Role           { get; set; }
        public DateTime JoinedDate     { get; set; }
        public bool     IsActive       { get; set; }
    }

    // Response — login success
    public class AuthResponse
    {
        public string Token    { get; set; }
        public string Username { get; set; }
        public string Role     { get; set; }
        public int    Id       { get; set; }
    }
}