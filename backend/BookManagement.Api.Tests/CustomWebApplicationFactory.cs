using BookManagement.Api.Data;
using BookManagement.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookManagement.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    public int AdminId { get; private set; }
    public int UserId { get; private set; }
    public int BookId1 { get; private set; }
    public int BookId2 { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // JWT for AuthService.GenerateJwt()
                ["Jwt:Key"] = "THIS_IS_A_TEST_KEY_12345678901234567890123456789012",
                ["Jwt:Issuer"] = "BookManagement.Api",
                ["Jwt:Audience"] = "BookManagement.Client",
                ["Jwt:ExpiresMinutes"] = "60",

                
                ["Admin:Username"] = "admin",
                ["Admin:Password"] = "Admin@123",
                ["DefaultUser:Username"] = "user1",
                ["DefaultUser:Password"] = "User@123"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_connection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            db.Database.EnsureCreated();

            DbSeeder.SeedAsync(db, config).GetAwaiter().GetResult();
           
            AdminId = db.Users.Single(u => u.Username == "admin").Id;
            UserId = db.Users.Single(u => u.Username == "user1").Id;
  
            BookId1 = db.Books.OrderBy(b => b.Id).First().Id;
            BookId2 = db.Books.OrderBy(b => b.Id).Skip(1).First().Id;
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
