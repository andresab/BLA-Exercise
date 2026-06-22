using FluentValidation;
using TaskManagement.API.Controllers;

namespace TaskManagement.API.Auth;

public sealed class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
