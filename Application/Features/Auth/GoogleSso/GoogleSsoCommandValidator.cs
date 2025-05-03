using FluentValidation;

namespace AuthImplementation.Application.Features.Auth.GoogleSso;

public class GoogleSsoCommandValidator : AbstractValidator<GoogleSsoCommand>
{
    public GoogleSsoCommandValidator()
    {
        RuleFor(x => x.AuthorizationCode)
            .NotEmpty().WithMessage("Authorization code is required.");
    }
}