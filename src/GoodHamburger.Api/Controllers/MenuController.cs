using GoodHamburger.Domain.DTOs;
using GoodHamburger.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }
    
    // GET api/menu - retorna card\u00e1pio completo
    [HttpGet]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenu()
    {
        var menuItems = await _menuService.GetAllMenuItemsAsync();
        return Ok(menuItems);
    }
}
