namespace GoodHamburger.Domain.DTOs;

public record AuthResponseDto(
    string Token,
    string Email,
    DateTime ExpiresAt
);
