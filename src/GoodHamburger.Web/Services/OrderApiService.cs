using System.Net.Http.Json;
using System.Net.Http.Headers;
using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public class OrderApiService : IOrderApiService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    
    public OrderApiService(HttpClient httpClient, ITokenService tokenService, IAuthService authService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _authService = authService;
    }
    
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = _tokenService.Token;

        // se não tem token em memória, busca do storage
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
    
    public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var orders = await _httpClient.GetFromJsonAsync<List<OrderResponseDto>>("api/orders");
            return orders ?? new List<OrderResponseDto>();
        }
        catch
        {
            return new List<OrderResponseDto>();
        }
    }
    
    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<OrderResponseDto>($"api/orders/{id}");
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<OrderResponseDto?> CreateOrderAsync(CreateOrderDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/orders", dto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<OrderResponseDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/orders/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
