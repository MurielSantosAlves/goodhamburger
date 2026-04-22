using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Domain.Services;

public interface IOrderService
{
    Task<OrderResponseDto?> GetOrderByIdAsync(int id, string userId);
    Task<List<OrderResponseDto>> GetAllOrdersByUserAsync(string userId);
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, string userId);
    Task<OrderResponseDto> UpdateOrderAsync(int id, UpdateOrderDto dto, string userId);
    Task DeleteOrderAsync(int id, string userId);
}
