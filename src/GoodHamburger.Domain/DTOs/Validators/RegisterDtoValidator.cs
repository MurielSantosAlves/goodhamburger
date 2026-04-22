using FluentValidation;

namespace GoodHamburger.Domain.DTOs.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
        
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("As senhas não coincidem");
    }
}
