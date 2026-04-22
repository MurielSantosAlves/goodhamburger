using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GoodHamburger.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private const string TokenKey = "authToken";
    private string? _cachedToken;
    
    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }
    
    public string? GetCachedToken() => _cachedToken;
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var tokenResult = await _sessionStorage.GetAsync<string>(TokenKey);
            
            if (!tokenResult.Success || string.IsNullOrWhiteSpace(tokenResult.Value))
            {
                _cachedToken = null;
                return new AuthenticationState(_anonymous);
            }
            
            // mantém token em cache para evitar múltiplas leituras do storage
            _cachedToken = tokenResult.Value;
            var claimsPrincipal = CreateClaimsPrincipalFromToken(tokenResult.Value);
            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            _cachedToken = null;
            return new AuthenticationState(_anonymous);
        }
    }
    
    public async Task NotifyUserAuthenticationAsync(string token)
    {
        _cachedToken = token;
        var claimsPrincipal = CreateClaimsPrincipalFromToken(token);
        var authState = Task.FromResult(new AuthenticationState(claimsPrincipal));
        NotifyAuthenticationStateChanged(authState);
        await Task.CompletedTask;
    }
    
    public async Task NotifyUserLogoutAsync()
    {
        _cachedToken = null;
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
        await Task.CompletedTask;
    }
    
    private ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return _anonymous;
        }
    }
}
