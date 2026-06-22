namespace TaskManagement.Application.Auth;

public interface IAuthService
{
    Task<TokenResponse> CreateTokenAsync(TokenRequest request, CancellationToken cancellationToken = default);
}
