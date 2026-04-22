using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Services;

public class DiscountCalculator : IDiscountCalculator
{
    public decimal CalculateDiscountPercentage(Order order)
    {
        var hasSandwich = order.Items.Any(i => i.MenuItem.Type == MenuItemType.Sandwich);
        var hasSideDish = order.Items.Any(i => i.MenuItem.Type == MenuItemType.SideDish);
        var hasDrink = order.Items.Any(i => i.MenuItem.Type == MenuItemType.Drink);
        
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
