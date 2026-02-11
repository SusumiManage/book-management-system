using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BookManagement.Api.Tests;

public class BookBorrowedDetailsIntegrationTests
{
    [Fact]
    public async Task GetActiveBorrows_WithoutAuthToken_ShouldReturnUnauthorized()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var res = await client.GetAsync("/api/BookBorrowedDetails/active");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BorrowBook_WithAdminToken_ShouldReturnOk_AndThenActiveListShouldIncludeBorrow()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var adminToken = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, adminToken);

        var borrowRes = await client.PostAsJsonAsync("/api/BookBorrowedDetails/borrow", new
        {
            bookId = factory.BookId1,
            borrowedByUserId = factory.UserId,
            dueAt = (string?)null
        });

        if (borrowRes.StatusCode == HttpStatusCode.BadRequest)
            throw new Exception("Borrow failed: " + await borrowRes.Content.ReadAsStringAsync());

        borrowRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var activeRes = await client.GetAsync("/api/BookBorrowedDetails/active");
        activeRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BorrowBook_SameBookBorrowedTwice_ShouldReturnBadRequestOnSecondBorrow()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var adminToken = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, adminToken);

        var payload = new
        {
            bookId = factory.BookId1,
            borrowedByUserId = factory.UserId,
            dueAt = (string?)null
        };

        var r1 = await client.PostAsJsonAsync("/api/BookBorrowedDetails/borrow", payload);
        if (r1.StatusCode == HttpStatusCode.BadRequest)
            throw new Exception("First borrow failed: " + await r1.Content.ReadAsStringAsync());
        r1.StatusCode.Should().Be(HttpStatusCode.OK);

        var r2 = await client.PostAsJsonAsync("/api/BookBorrowedDetails/borrow", payload);
        r2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnBook_AfterBorrow_ShouldReturnNoContent_AndActiveListShouldNotContainThatBook()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var adminToken = await AuthTestHelper.LoginAndGetTokenAsync(client, "admin", "Admin@123");
        AuthTestHelper.SetBearer(client, adminToken);

        var bookId = factory.BookId1;

        var borrowRes = await client.PostAsJsonAsync("/api/BookBorrowedDetails/borrow", new
        {
            bookId,
            borrowedByUserId = factory.UserId,
            dueAt = (string?)null
        });

        if (borrowRes.StatusCode == HttpStatusCode.BadRequest)
            throw new Exception("Borrow failed: " + await borrowRes.Content.ReadAsStringAsync());

        borrowRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnRes = await client.PostAsJsonAsync("/api/BookBorrowedDetails/return", new { bookId });
        returnRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activeRes = await client.GetAsync("/api/BookBorrowedDetails/active");
        activeRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await activeRes.Content.ReadAsStringAsync();
        json.Should().NotContain($"\"bookId\":{bookId}");
    }
}
