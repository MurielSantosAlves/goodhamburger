using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public interface IDiscountRuleApiService
{
    Task<List<DiscountRuleDto>> GetAllAsync();
    Task<List<DiscountRuleDto>> GetActiveAsync();
    Task<DiscountRuleDto?> GetByIdAsync(int id);
    Task<DiscountRuleDto?> CreateAsync(CreateDiscountRuleDto dto);
    Task<DiscountRuleDto?> UpdateAsync(int id, UpdateDiscountRuleDto dto);
    Task<bool> DeleteAsync(int id);
}
