using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Web.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;
    
    public AuthenticationDelegatingHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var authStateProvider = scope.ServiceProvider.GetRequiredService<CustomAuthenticationStateProvider>();
        var token = authStateProvider.GetCachedToken();
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}
