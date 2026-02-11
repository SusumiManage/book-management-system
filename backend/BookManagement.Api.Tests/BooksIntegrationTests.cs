using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BookManagement.Api.Tests;

public class BooksIntegrationTests
{
    [Fact]
    public async Task GetBooks_WithoutAuthenticationToken_ShouldReturnUnauthorized()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var res = await client.GetAsync("/api/books");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBooks_WithValidUserToken_ShouldReturnOk()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginAndGetTokenAsync(client, "user1", "User@123");
        AuthTestHelper.SetBearer(client, token);

        var res = await client.GetAsync("/api/books?pageNumber=1&pageSize=5");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBook_WithAdminToken_ShouldReturnCreated()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, token);

        var payload = new
        {
            title = "Test Book",
            author = "Tester",
            genre = "Programming",
            publicationYear = 2008,
            isbn = Guid.NewGuid().ToString("N")[..20],
            price = 39.99m
        };

        var res = await client.PostAsJsonAsync("/api/books", payload);

        if (res.StatusCode == HttpStatusCode.BadRequest)
            throw new Exception("Create failed: " + await res.Content.ReadAsStringAsync());

        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateBook_WithUserToken_ShouldReturnForbidden()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginAndGetTokenAsync(client, "user1", "User@123");
        AuthTestHelper.SetBearer(client, token);

        var payload = new
        {
            title = "User Book",
            author = "Someone",
            genre = "Test",
            publicationYear = 2020,
            isbn = "ISBN-" + Guid.NewGuid().ToString("N")[..20],
            price = 10m
        };

        var res = await client.PostAsJsonAsync("/api/books", payload);
        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBook_WithDuplicateIsbn_ShouldReturnBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, token);

        var dupIsbn = ("DUP" + Guid.NewGuid().ToString("N"))[..20];

        var payload = new
        {
            title = "Book 1",
            author = "A",
            genre = "G",
            publicationYear = 2020,
            isbn = dupIsbn,
            price = 10m
        };

        (await client.PostAsJsonAsync("/api/books", payload)).StatusCode.Should().Be(HttpStatusCode.Created);

        var res2 = await client.PostAsJsonAsync("/api/books", new
        {
            title = "Book 2",
            author = "B",
            genre = "G",
            publicationYear = 2021,
            isbn = dupIsbn,
            price = 12m
        });

        res2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBook_ThenRestoreBook_WithAdminToken_ShouldSucceed()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, token);

        var isbn = ("DEL" + Guid.NewGuid().ToString("N"))[..20];

        // Create
        var createRes = await client.PostAsJsonAsync("/api/books", new
        {
            title = "To Delete",
            author = "X",
            genre = "Y",
            publicationYear = 2022,
            isbn = isbn,
            price = 9.99m
        });

        if (createRes.StatusCode == HttpStatusCode.BadRequest)
            throw new Exception("Create failed: " + await createRes.Content.ReadAsStringAsync());

        createRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdJson = await createRes.Content.ReadAsStringAsync();
        var id = JsonDocument.Parse(createdJson).RootElement.GetProperty("id").GetInt32();

        // Delete 
        var delRes = await client.DeleteAsync($"/api/books/{id}");
        delRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Restore
        var restoreRes = await client.PostAsync($"/api/books/{id}/restore", JsonContent.Create(new { }));
        restoreRes.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
