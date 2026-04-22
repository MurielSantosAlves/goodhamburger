namespace GoodHamburger.Domain.DTOs;

public record RegisterDto(
    string Email,
    string Password,
    string ConfirmPassword
);
