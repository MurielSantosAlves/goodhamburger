using System.Net.Http.Json;
using System.Net.Http.Headers;
using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public class MenuApiService : IMenuApiService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    
    public MenuApiService(HttpClient httpClient, ITokenService tokenService, IAuthService authService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _authService = authService;
    }
    
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = _tokenService.Token;

        if (string.IsNullOrWhiteSpace(token))
        {
            token = await _authService.GetTokenAsync();
            _tokenService.Token = token;
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
    
    public async Task<List<MenuItemDto>> GetMenuAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var menuItems = await _httpClient.GetFromJsonAsync<List<MenuItemDto>>("api/menu");
            return menuItems ?? new List<MenuItemDto>();
        }
        catch
        {
            return new List<MenuItemDto>();
        }
    }
}
