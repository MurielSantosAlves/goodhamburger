using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Domain.Services;

public interface IMenuService
{
    Task<List<MenuItemDto>> GetAllMenuItemsAsync();
}
