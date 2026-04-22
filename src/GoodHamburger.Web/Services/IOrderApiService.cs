using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public interface IOrderApiService
{
    Task<List<OrderResponseDto>> GetAllOrdersAsync();
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<OrderResponseDto?> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);
    Task<bool> DeleteOrderAsync(int id);
}
