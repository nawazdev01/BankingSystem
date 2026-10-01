using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BankingSystem.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateStaffToken(Staff staff)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
                new Claim(ClaimTypes.Name,           staff.Username),
                new Claim(ClaimTypes.Role,           staff.Role.ToString()),
                new Claim("EmployeeNumber",          staff.EmployeeNumber),
                new Claim("UserType",                "Staff")
            };

            return GenerateToken(claims);
        }

        public string GenerateCustomerToken(Customer customer)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name,           customer.Username),
                new Claim("CustomerNumber",          customer.CustomerNumber),
                new Claim("UserType",                "Customer")
            };

            return GenerateToken(claims);
        }

        private string GenerateToken(Claim[] claims)
        {
            var secretKey  = _config["JwtSettings:SecretKey"];
            var issuer     = _config["JwtSettings:Issuer"];
            var audience   = _config["JwtSettings:Audience"];
            var expiryDays = int.Parse(_config["JwtSettings:ExpiryDays"]);

            var key   = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(
                            key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                expires:            DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}