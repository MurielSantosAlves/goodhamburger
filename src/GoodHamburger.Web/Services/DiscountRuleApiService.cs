using System.Net.Http.Json;
using System.Net.Http.Headers;
using GoodHamburger.Domain.DTOs;

namespace GoodHamburger.Web.Services;

public class DiscountRuleApiService : IDiscountRuleApiService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    
    public DiscountRuleApiService(HttpClient httpClient, ITokenService tokenService, IAuthService authService)
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
    
    public async Task<List<DiscountRuleDto>> GetAllAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var rules = await _httpClient.GetFromJsonAsync<List<DiscountRuleDto>>("api/discountrules");
            return rules ?? new List<DiscountRuleDto>();
        }
        catch
        {
            return new List<DiscountRuleDto>();
        }
    }
    
    public async Task<List<DiscountRuleDto>> GetActiveAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var rules = await _httpClient.GetFromJsonAsync<List<DiscountRuleDto>>("api/discountrules/active");
            return rules ?? new List<DiscountRuleDto>();
        }
        catch
        {
            return new List<DiscountRuleDto>();
        }
    }
    
    public async Task<DiscountRuleDto?> GetByIdAsync(int id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<DiscountRuleDto>($"api/discountrules/{id}");
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<DiscountRuleDto?> CreateAsync(CreateDiscountRuleDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/discountrules", dto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DiscountRuleDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<DiscountRuleDto?> UpdateAsync(int id, UpdateDiscountRuleDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/discountrules/{id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DiscountRuleDto>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/discountrules/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
