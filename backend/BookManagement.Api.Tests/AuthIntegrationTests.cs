using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BookManagement.Api.Tests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidAdminCredentials_ShouldReturnOkAndToken()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "Admin@123" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await res.Content.ReadAsStringAsync();
        body.Should().Contain("token");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "wrong" });
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithoutAuthenticationToken_ShouldReturnUnauthorized()
    {
        var res = await _client.GetAsync("/api/auth/users");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
