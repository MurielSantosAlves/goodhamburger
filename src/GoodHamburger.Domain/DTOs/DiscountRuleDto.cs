namespace GoodHamburger.Domain.DTOs;

public class DiscountRuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public bool RequiresSandwich { get; set; }
    public bool RequiresSideDish { get; set; }
    public bool RequiresDrink { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
