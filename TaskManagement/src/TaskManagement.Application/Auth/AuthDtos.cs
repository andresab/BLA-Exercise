namespace TaskManagement.Application.Auth;

public sealed record TokenRequest(string Email);

public sealed record TokenResponse(string AccessToken);
