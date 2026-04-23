using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Domain.DTOs;

public class CreateDiscountRuleDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O percentual de desconto é obrigatório")]
    [Range(0, 1, ErrorMessage = "O percentual deve estar entre 0 e 1 (0% a 100%)")]
    public decimal DiscountPercentage { get; set; }
    
    public bool RequiresSandwich { get; set; }
    public bool RequiresSideDish { get; set; }
    public bool RequiresDrink { get; set; }
    public bool IsActive { get; set; } = true;
    
    [Required(ErrorMessage = "A prioridade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "A prioridade deve ser maior que 0")]
    public int Priority { get; set; }
}
