using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.Repositories;

public class DiscountRuleRepository : IDiscountRuleRepository
{
    private readonly ApplicationDbContext _context;
    
    public DiscountRuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<DiscountRule>> GetAllActiveAsync()
    {
        return await _context.DiscountRules
            .Where(dr => dr.IsActive)
            .OrderBy(dr => dr.Priority)
            .ToListAsync();
    }
    
    public async Task<List<DiscountRule>> GetAllAsync()
    {
        return await _context.DiscountRules
            .OrderBy(dr => dr.Priority)
            .ToListAsync();
    }
    
    public async Task<DiscountRule?> GetByIdAsync(int id)
    {
        return await _context.DiscountRules.FindAsync(id);
    }
    
    public async Task<DiscountRule> CreateAsync(DiscountRule discountRule)
    {
        _context.DiscountRules.Add(discountRule);
        await _context.SaveChangesAsync();
        return discountRule;
    }
    
    public async Task<DiscountRule> UpdateAsync(DiscountRule discountRule)
    {
        discountRule.UpdatedAt = DateTime.UtcNow;
        _context.DiscountRules.Update(discountRule);
        await _context.SaveChangesAsync();
        return discountRule;
    }
    
    public async Task DeleteAsync(int id)
    {
        var discountRule = await GetByIdAsync(id);
        if (discountRule != null)
        {
            _context.DiscountRules.Remove(discountRule);
            await _context.SaveChangesAsync();
        }
    }
}
