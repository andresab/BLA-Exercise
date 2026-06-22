using TaskManagement.Application.Common;
using TaskManagement.Application.Users;

namespace TaskManagement.Application.Auth;

public sealed class AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<TokenResponse> CreateTokenAsync(TokenRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", normalizedEmail);
        }

        return new TokenResponse(jwtTokenGenerator.CreateToken(user));
    }
}
