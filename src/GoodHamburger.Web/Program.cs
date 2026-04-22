using System.Globalization;
using GoodHamburger.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// data protection - mantém chaves entre restarts
var keysPath = Path.Combine("/app", "keys");
Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("GoodHamburger");

// componentes Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// autenticação e autorização
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<ITokenService, TokenService>();

// configura HttpClient para autenticação (sem DelegatingHandler)
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// HttpClient para API de menu
builder.Services.AddHttpClient<IMenuApiService, MenuApiService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// HttpClient para API de pedidos
builder.Services.AddHttpClient<IOrderApiService, OrderApiService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configurar cultura brasileira para exibir R$ corretamente
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
