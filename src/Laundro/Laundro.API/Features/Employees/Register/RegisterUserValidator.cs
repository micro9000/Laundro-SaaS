using FastEndpoints;

namespace Laundro.API.Features.Employees.Register;

internal class RegisterUserValidator : Validator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Employee name is required")
            .MinimumLength(3).WithMessage("Employee name is too short!");

        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty().WithMessage("Employee email is required")
            .MinimumLength(6).WithMessage("Employee email is too short!");
    }
}