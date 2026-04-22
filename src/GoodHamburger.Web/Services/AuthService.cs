using System.Net.Http.Json;
using GoodHamburger.Domain.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GoodHamburger.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ITokenService _tokenService;
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private const string TokenKey = "authToken";
    
    public AuthService(
        HttpClient httpClient,
        ProtectedSessionStorage sessionStorage,
        ITokenService tokenService,
        CustomAuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _sessionStorage = sessionStorage;
        _tokenService = tokenService;
        _authStateProvider = authStateProvider;
    }
    
    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse != null)
                {
                    await _sessionStorage.SetAsync(TokenKey, authResponse.Token);
                    _tokenService.Token = authResponse.Token;
                    await _authStateProvider.NotifyUserAuthenticationAsync(authResponse.Token);
                    return new AuthResult(true, authResponse, null);
                }
            }
            
            var errorMessage = await ExtractErrorMessage(response);
            return new AuthResult(false, null, errorMessage);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, null, $"Erro ao conectar com o servidor: {ex.Message}");
        }
    }
    
    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse != null)
                {
                    await _sessionStorage.SetAsync(TokenKey, authResponse.Token);
                    _tokenService.Token = authResponse.Token;
                    await _authStateProvider.NotifyUserAuthenticationAsync(authResponse.Token);
                    return new AuthResult(true, authResponse, null);
                }
            }
            
            var errorMessage = await ExtractErrorMessage(response);
            return new AuthResult(false, null, errorMessage);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, null, $"Erro ao conectar com o servidor: {ex.Message}");
        }
    }
    
    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync(TokenKey);
        _tokenService.Token = null;
        await _authStateProvider.NotifyUserLogoutAsync();
    }
    
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
            {
                _tokenService.Token = result.Value;
                return result.Value;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    
    // extrai mensagem de erro da resposta da API
    // BadRequest retorna { errors: [] }, Unauthorized retorna { message: "" }
    private async Task<string> ExtractErrorMessage(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(content, 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Tenta extrair "message" primeiro (Unauthorized retorna { message: "..." })
            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
            {
                return errorResponse.Message;
            }
            
            // Tenta extrair "errors" (BadRequest retorna { errors: [...] })
            if (errorResponse?.Errors != null && errorResponse.Errors.Any())
            {
                return string.Join(", ", errorResponse.Errors);
            }
            
            return $"Erro: {response.StatusCode}";
        }
        catch
        {
            return $"Erro: {response.StatusCode}";
        }
    }
    
    private record ErrorResponse(string? Message, string[]? Errors);
}
