using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class DiscountRule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public bool RequiresSandwich { get; set; }
    public bool RequiresSideDish { get; set; }
    public bool RequiresDrink { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
