using AuthImplementation.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public LoginCommandValidator(UserManager<AppUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x)
            .MustAsync(BeAValidUser).WithMessage("Invalid credentials").WithName("Email");
    }

    private async Task<bool> BeAValidUser(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return false;

        return await _userManager.CheckPasswordAsync(user, command.Password);
    }
}