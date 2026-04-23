using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Domain.Services;

public class DiscountCalculator : IDiscountCalculator
{
    private readonly IDiscountRuleRepository _discountRuleRepository;
    
    public DiscountCalculator(IDiscountRuleRepository discountRuleRepository)
    {
        _discountRuleRepository = discountRuleRepository;
    }
    
    public decimal CalculateDiscountPercentage(Order order)
    {
        var hasSandwich = order.Items.Any(i => i.MenuItem.Type == MenuItemType.Sandwich);
        var hasSideDish = order.Items.Any(i => i.MenuItem.Type == MenuItemType.SideDish);
        var hasDrink = order.Items.Any(i => i.MenuItem.Type == MenuItemType.Drink);
        
        // Busca todas as regras ativas ordenadas por prioridade
        var rules = _discountRuleRepository.GetAllActiveAsync().Result;
        
        // Aplica a primeira regra que satisfaz as condições
        foreach (var rule in rules)
        {
            var matchesSandwich = !rule.RequiresSandwich || hasSandwich;
            var matchesSideDish = !rule.RequiresSideDish || hasSideDish;
            var matchesDrink = !rule.RequiresDrink || hasDrink;
            
            if (matchesSandwich && matchesSideDish && matchesDrink)
            {
                return rule.DiscountPercentage;
            }
        }
        
        return 0m;
    }
}
