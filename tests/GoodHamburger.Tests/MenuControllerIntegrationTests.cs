using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GoodHamburger.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GoodHamburger.Tests.Integration;

public class MenuControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MenuControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var email = $"menutest{Guid.NewGuid()}@example.com";
        var password = "Password@123";
        
        var registerDto = new RegisterDto(email, password, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        return authResult!.Token;
    }

    [Fact]
    public async Task GetMenu_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/menu");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMenu_WithAuth_ShouldReturnMenuItems()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/menu");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        Assert.NotNull(menuItems);
        Assert.True(menuItems.Count >= 5); // At least the seeded items
        
        // Verify we have all types
        Assert.Contains(menuItems, m => m.Type == Domain.Enums.MenuItemType.Sandwich);
        Assert.Contains(menuItems, m => m.Type == Domain.Enums.MenuItemType.SideDish);
        Assert.Contains(menuItems, m => m.Type == Domain.Enums.MenuItemType.Drink);
    }

    [Fact]
    public async Task GetMenu_WithAuth_ShouldContainSeededItems()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/menu");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        Assert.NotNull(menuItems);
        
        // Verify seeded items exist
        Assert.Contains(menuItems, m => m.Name.Contains("X Burger"));
        Assert.Contains(menuItems, m => m.Name.Contains("Batata frita"));
        Assert.Contains(menuItems, m => m.Name.Contains("Refrigerante"));
    }

    [Fact]
    public async Task GetMenu_WithAuth_AllItemsShouldHaveValidPrices()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/menu");

        // Assert
        var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        Assert.NotNull(menuItems);
        Assert.All(menuItems, item => Assert.True(item.Price > 0));
    }
}
