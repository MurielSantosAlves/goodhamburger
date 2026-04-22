using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public ApplicationUser User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    
    // Propriedades calculadas
    public decimal SubTotal => Items.Sum(i => i.Price);
    
    public decimal DiscountPercentage
    {
        get
        {
            var hasSandwich = Items.Any(i => i.MenuItem.Type == MenuItemType.Sandwich);
            var hasSideDish = Items.Any(i => i.MenuItem.Type == MenuItemType.SideDish);
            var hasDrink = Items.Any(i => i.MenuItem.Type == MenuItemType.Drink);
            
            // Sanduíche + batata + refrigerante → 20% de desconto
            if (hasSandwich && hasSideDish && hasDrink)
                return 0.20m;
            
            // Sanduíche + refrigerante → 15% de desconto
            if (hasSandwich && hasDrink)
                return 0.15m;
            
            // Sanduíche + batata → 10% de desconto
            if (hasSandwich && hasSideDish)
                return 0.10m;
            
            return 0m;
        }
    }
    
    public decimal DiscountAmount => SubTotal * DiscountPercentage;
    
    public decimal Total => SubTotal - DiscountAmount;
}
