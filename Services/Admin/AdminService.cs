using be.Data;
using be.Models;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Admin;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AssignRoleAsync(Guid userId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("Role is required.");
        }

        roleName = roleName.Trim().ToUpperInvariant();

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            throw new ArgumentException("Role not found.");
        }

        if (user.UserRoles.Count == 1 &&
            user.UserRoles.Any(ur => ur.RoleId == role.Id))
        {
            return;
        }

        _context.UserRoles.RemoveRange(user.UserRoles);

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _context.SaveChangesAsync();
    }
}