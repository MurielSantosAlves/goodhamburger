using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GoodHamburger.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GoodHamburger.Tests.Integration;

public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var email = $"ordertest{Guid.NewGuid()}@example.com";
        var password = "Password@123";
        
        var registerDto = new RegisterDto(email, password, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        return authResult!.Token;
    }

    [Fact]
    public async Task GetOrders_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/orders");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrders_WithAuth_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/orders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateOrderDto(new List<int> { 1, 4, 5 }); // Sandwich, SideDish, Drink for full combo

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        Assert.NotNull(order);
        Assert.Equal(3, order.Items.Count);
        Assert.Equal(0.20m, order.DiscountPercentage); // Full combo discount
    }

    [Fact]
    public async Task CreateOrder_WithInvalidMenuItems_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateOrderDto(new List<int> { 999 }); // Non-existent menu item

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrder_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create order first with full combo
        var createDto = new CreateOrderDto(new List<int> { 1, 4, 5 });
        var createResponse = await _client.PostAsJsonAsync("/api/orders", createDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Update order to Sandwich + Drink (remove SideDish)
        var updateDto = new UpdateOrderDto(new List<int> { 1, 5 });

        // Act
        var response = await _client.PutAsJsonAsync($"/api/orders/{createdOrder!.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedOrder = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        Assert.NotNull(updatedOrder);
        Assert.Equal(2, updatedOrder.Items.Count);
        Assert.Equal(0.15m, updatedOrder.DiscountPercentage); // Sandwich + Drink discount
    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create order first
        var createDto = new CreateOrderDto(new List<int> { 1 });
        var createResponse = await _client.PostAsJsonAsync("/api/orders", createDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/orders/{createdOrder!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/orders/{createdOrder.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetOrderById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/orders/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithDuplicateMenuItemTypes_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Try to add two sandwiches (IDs 1 and 2 are both sandwiches)
        var createDto = new CreateOrderDto(new List<int> { 1, 2 });

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
