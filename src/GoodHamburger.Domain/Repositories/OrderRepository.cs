using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    
    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Order?> GetByIdAsync(int id, string userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
    }
    
    public async Task<List<Order>> GetAllByUserAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
    
    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        // Recarregar com includes
        return (await GetByIdAsync(order.Id, order.UserId))!;
    }
    
    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        
        // Recarregar com includes
        return (await GetByIdAsync(order.Id, order.UserId))!;
    }
    
    public async Task DeleteAsync(int id, string userId)
    {
        var order = await GetByIdAsync(id, userId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
