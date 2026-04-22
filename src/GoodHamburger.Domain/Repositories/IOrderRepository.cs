using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, string userId);
    Task<List<Order>> GetAllByUserAsync(string userId);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task DeleteAsync(int id, string userId);
}
