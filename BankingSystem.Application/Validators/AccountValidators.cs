using BankingSystem.Application.DTOs;
using BankingSystem.Domain.Enums;
using FluentValidation;

namespace BankingSystem.Application.Validators
{
    public class CreateAccountRequestValidator
        : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Valid customer is required");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid account type");

            RuleFor(x => x.InitialDeposit)
                .GreaterThan(0).WithMessage("Initial deposit must be greater than zero")
                .LessThanOrEqualTo(1000000).WithMessage("Initial deposit cannot exceed 1,000,000");

            RuleFor(x => x.InterestRate)
                .GreaterThan(0).WithMessage("Interest rate must be positive")
                .LessThanOrEqualTo(100).WithMessage("Interest rate cannot exceed 100%")
                .When(x => x.Type == AccountType.Savings ||
                           x.Type == AccountType.FixedDeposit);

            RuleFor(x => x.OverdraftLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Overdraft limit cannot be negative")
                .When(x => x.Type == AccountType.Current);

            RuleFor(x => x.MaturityDate)
                .NotNull().WithMessage("Maturity date is required for Fixed Deposit")
                .Must(date => date > DateTime.Today)
                    .WithMessage("Maturity date must be in the future")
                .When(x => x.Type == AccountType.FixedDeposit);
        }
    }
}