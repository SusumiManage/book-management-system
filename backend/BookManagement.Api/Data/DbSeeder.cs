using BookManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Data
{
    /// <summary>
    /// Seeds default users and books (only if missing).
    /// Also applies database migrations when using SQL Server.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, IConfiguration config)
        {
            //await db.Database.MigrateAsync();
            // Only migrate for SQL Server (real app)
            if (db.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
                await db.Database.MigrateAsync();

            await SeedUsersAsync(db, config);
            await SeedBooksAsync(db);

            if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var adminSection = config.GetSection("Admin");
                var username = adminSection["Username"] ?? "admin";
                var password = adminSection["Password"] ?? "Admin@123";

                var admin = new AppUser
                {
                    Username = username,
                    Role = "Admin"
                };

                var hasher = new PasswordHasher<AppUser>();
                admin.PasswordHash = hasher.HashPassword(admin, password);

                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedUsersAsync(AppDbContext db, IConfiguration config)
        {
            var hasher = new PasswordHasher<AppUser>();

            // --- Admin user ---
            if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var adminSection = config.GetSection("Admin");
                var username = adminSection["Username"] ?? "admin";
                var password = adminSection["Password"] ?? "Admin@123";

                var admin = new AppUser
                {
                    Username = username,
                    Role = "Admin"
                };

                admin.PasswordHash = hasher.HashPassword(admin, password);

                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }

            // --- Default normal user ---
            if (!await db.Users.AnyAsync(u => u.Role == "User"))
            {
                var userSection = config.GetSection("DefaultUser");
                var username = userSection["Username"] ?? "user";
                var password = userSection["Password"] ?? "User@123";

                var user = new AppUser
                {
                    Username = username,
                    Role = "User"
                };

                user.PasswordHash = hasher.HashPassword(user, password);

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedBooksAsync(AppDbContext db)
        {
            // Only seed books if table is empty
            if (await db.Books.AnyAsync())
                return;

            var books = new List<Book>
            {
                new Book { Title="Clean Code", Author="Robert C. Martin", Genre="Programming", PublicationYear=2008, ISBN="9780132350884", Price=40.00m },
                new Book { Title="The Pragmatic Programmer", Author="Andrew Hunt", Genre="Programming", PublicationYear=1999, ISBN="9780201616224", Price=45.00m },
                new Book { Title="Design Patterns", Author="Erich Gamma", Genre="Software Engineering", PublicationYear=1994, ISBN="9780201633610", Price=50.00m },
                new Book { Title="Refactoring", Author="Martin Fowler", Genre="Programming", PublicationYear=1999, ISBN="9780201485677", Price=42.00m },
                new Book { Title="Head First Design Patterns", Author="Eric Freeman", Genre="Software Engineering", PublicationYear=2004, ISBN="9780596007126", Price=44.99m },
                new Book { Title="C# in Depth", Author="Jon Skeet", Genre="C#", PublicationYear=2019, ISBN="9781617294536", Price=61.00m },
                new Book { Title="Effective C#", Author="Bill Wagner", Genre="C#", PublicationYear=2017, ISBN="9780135159941", Price=45.00m },
                new Book { Title="ASP.NET Core in Action", Author="Andrew Lock", Genre=".NET", PublicationYear=2021, ISBN="9781617298305", Price=49.99m },
                new Book { Title="Domain-Driven Design", Author="Eric Evans", Genre="Software Engineering", PublicationYear=2003, ISBN="9780321125217", Price=55.00m },
                new Book { Title="Working Effectively with Legacy Code", Author="Michael Feathers", Genre="Programming", PublicationYear=2004, ISBN="9780131177055", Price=48.00m }
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync();
        }
    }
}
