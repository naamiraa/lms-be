using be.DTOs.Auth;

namespace be.Services.Auth;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);
}