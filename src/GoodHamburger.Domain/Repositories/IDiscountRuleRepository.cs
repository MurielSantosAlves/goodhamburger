using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Repositories;

public interface IDiscountRuleRepository
{
    Task<List<DiscountRule>> GetAllActiveAsync();
    Task<List<DiscountRule>> GetAllAsync();
    Task<DiscountRule?> GetByIdAsync(int id);
    Task<DiscountRule> CreateAsync(DiscountRule discountRule);
    Task<DiscountRule> UpdateAsync(DiscountRule discountRule);
    Task DeleteAsync(int id);
}
