using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;
    
    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<MenuItemDto>> GetAllMenuItemsAsync()
    {
        var menuItems = await _context.MenuItems
            .OrderBy(m => m.Type)
            .ThenBy(m => m.Name)
            .ToListAsync();
        
        return menuItems.Select(m => new MenuItemDto(m.Id, m.Name, m.Price, m.Type)).ToList();
    }
}
