using be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace be.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        var adminEmail = configuration["BootstrapAdmin:Email"];
        var adminPassword = configuration["BootstrapAdmin:Password"];
        var adminName = configuration["BootstrapAdmin:Name"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var existingAdmin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin != null)
        {
            return;
        }

        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "ADMIN");

        if (adminRole == null)
        {
            throw new Exception("ADMIN role is not configured.");
        }

        var admin = new User
        {
            Name = adminName ?? "LMS Administrator",
            Email = adminEmail,
            RegistrationSource = "SYSTEM",
            ApprovalStatus = "APPROVED",
            IsActive = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            adminPassword
        );

        admin.UserRoles.Add(new UserRole
        {
            User = admin,
            Role = adminRole
        });

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}