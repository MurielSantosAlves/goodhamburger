using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ApplicationDbContext _context;
    
    public OrderService(IOrderRepository orderRepository, ApplicationDbContext context)
    {
        _orderRepository = orderRepository;
        _context = context;
    }
    
    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, string userId)
    {
        var order = await _orderRepository.GetByIdAsync(id, userId);
        return order == null ? null : MapToDto(order);
    }
    
    public async Task<List<OrderResponseDto>> GetAllOrdersByUserAsync(string userId)
    {
        var orders = await _orderRepository.GetAllByUserAsync(userId);
        return orders.Select(MapToDto).ToList();
    }
    
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, string userId)
    {
        // busca os itens do menu pelos IDs recebidos
        var menuItems = await _context.MenuItems
            .Where(m => dto.MenuItemIds.Contains(m.Id))
            .ToListAsync();
        
        // cria o pedido com os itens
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Items = menuItems.Select(m => new OrderItem
            {
                MenuItemId = m.Id,
                MenuItem = m,
                Price = m.Price
            }).ToList()
        };
        
        var createdOrder = await _orderRepository.CreateAsync(order);
        return MapToDto(createdOrder);
    }
    
    public async Task<OrderResponseDto> UpdateOrderAsync(int id, UpdateOrderDto dto, string userId)
    {
        var order = await _orderRepository.GetByIdAsync(id, userId);
        if (order == null)
            throw new InvalidOperationException("Pedido não encontrado");
        
        // pega os novos itens do cardápio
        var menuItems = await _context.MenuItems
            .Where(m => dto.MenuItemIds.Contains(m.Id))
            .ToListAsync();
        
        // limpa itens antigos e adiciona os novos
        order.Items.Clear();
        
        foreach (var menuItem in menuItems)
        {
            order.Items.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                MenuItem = menuItem,
                Price = menuItem.Price,
                OrderId = order.Id
            });
        }
        
        var updatedOrder = await _orderRepository.UpdateAsync(order);
        return MapToDto(updatedOrder);
    }
    
    public async Task DeleteOrderAsync(int id, string userId)
    {
        await _orderRepository.DeleteAsync(id, userId);
    }
    
    private static OrderResponseDto MapToDto(Order order)
    {
        return new OrderResponseDto(
            order.Id,
            order.OrderDate,
            order.Items.Select(i => new OrderItemDto(i.Id, i.MenuItem.Name, i.Price)).ToList(),
            order.SubTotal,
            order.DiscountPercentage,
            order.DiscountAmount,
            order.Total
        );
    }
}
