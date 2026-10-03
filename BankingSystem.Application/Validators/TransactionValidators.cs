using BankingSystem.Application.DTOs;
using FluentValidation;

namespace BankingSystem.Application.Validators
{
    public class DepositRequestValidator
        : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Valid account is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Deposit amount must be greater than zero")
                .LessThanOrEqualTo(1000000).WithMessage("Deposit cannot exceed 1,000,000");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
        }
    }

    public class WithdrawRequestValidator
        : AbstractValidator<WithdrawRequest>
    {
        public WithdrawRequestValidator()
        {
            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Valid account is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Withdrawal amount must be greater than zero")
                .LessThanOrEqualTo(1000000).WithMessage("Withdrawal cannot exceed 1,000,000");
        }
    }

    public class TransferRequestValidator
        : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.SenderAccountId)
                .GreaterThan(0).WithMessage("Valid sender account is required");

            RuleFor(x => x.ReceiverAccountId)
                .GreaterThan(0).WithMessage("Valid receiver account is required")
                .NotEqual(x => x.SenderAccountId)
                    .WithMessage("Cannot transfer to the same account");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Transfer amount must be greater than zero")
                .LessThanOrEqualTo(1000000).WithMessage("Transfer cannot exceed 1,000,000");
        }
    }
}