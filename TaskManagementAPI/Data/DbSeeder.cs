using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class DbSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext db, IConfiguration configuration)
        {
            var adminExists = await db.Users.FirstOrDefaultAsync(x => x.Role == UserRole.Admin);

            if (adminExists!=null)
                return;

            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
                throw new Exception("Admin credentials are not configured.");

            var admin = new User
            {
                Name = "Laxmi",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.Admin
            };

            await db.Users.AddAsync(admin);
            await db.SaveChangesAsync();
        }
    }
}
