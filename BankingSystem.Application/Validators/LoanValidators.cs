using BankingSystem.Application.DTOs;
using FluentValidation;

namespace BankingSystem.Application.Validators
{
    public class ApplyLoanRequestValidator
        : AbstractValidator<ApplyLoanRequest>
    {
        public ApplyLoanRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Valid customer is required");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Valid account is required");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid loan type");

            RuleFor(x => x.LoanAmount)
                .GreaterThan(0).WithMessage("Loan amount must be greater than zero")
                .LessThanOrEqualTo(10000000).WithMessage("Loan cannot exceed 10,000,000");

            RuleFor(x => x.TermMonths)
                .GreaterThan(0).WithMessage("Term must be greater than zero months")
                .LessThanOrEqualTo(360).WithMessage("Term cannot exceed 360 months (30 years)");

            RuleFor(x => x.Purpose)
                .NotEmpty().WithMessage("Loan purpose is required")
                .MaximumLength(500).WithMessage("Purpose cannot exceed 500 characters");
        }
    }

    public class ApproveLoanRequestValidator
        : AbstractValidator<ApproveLoanRequest>
    {
        public ApproveLoanRequestValidator()
        {
            RuleFor(x => x.ApprovedAmount)
                .GreaterThan(0).WithMessage("Approved amount must be greater than zero");

            RuleFor(x => x.InterestRate)
                .GreaterThan(0).WithMessage("Interest rate must be greater than zero")
                .LessThanOrEqualTo(100).WithMessage("Interest rate cannot exceed 100%");
        }
    }

    public class RejectLoanRequestValidator
        : AbstractValidator<RejectLoanRequest>
    {
        public RejectLoanRequestValidator()
        {
            RuleFor(x => x.RejectionReason)
                .NotEmpty().WithMessage("Rejection reason is required")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
        }
    }

    public class LoanRepaymentRequestValidator
        : AbstractValidator<LoanRepaymentRequest>
    {
        public LoanRepaymentRequestValidator()
        {
            RuleFor(x => x.LoanId)
                .GreaterThan(0).WithMessage("Valid loan is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Repayment amount must be greater than zero");
        }
    }
}