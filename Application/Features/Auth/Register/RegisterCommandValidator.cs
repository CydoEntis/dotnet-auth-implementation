using AuthImplementation.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterCommandValidator(UserManager<AppUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email already in use");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken token)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser is null;
    }
}