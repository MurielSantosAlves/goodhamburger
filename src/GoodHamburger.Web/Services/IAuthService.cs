using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public record AuthResult(bool Success, AuthResponseDto? Response, string? ErrorMessage);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
}
