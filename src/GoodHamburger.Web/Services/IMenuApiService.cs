using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public interface IMenuApiService
{
    Task<List<MenuItemDto>> GetMenuAsync();
}
