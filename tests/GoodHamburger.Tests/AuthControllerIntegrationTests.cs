using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GoodHamburger.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GoodHamburger.Tests.Integration;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var registerDto = new RegisterDto(
            $"test{Guid.NewGuid()}@example.com",
            "Password@123",
            "Password@123"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var registerDto = new RegisterDto(email, "Password@123", "Password@123");

        // First registration
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Act - Try to register again with same email
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto(
            $"test{Guid.NewGuid()}@example.com",
            "Password@123",
            "DifferentPassword@123"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Password@123";
        
        var registerDto = new RegisterDto(email, password, password);
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent@example.com", "WrongPassword@123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
