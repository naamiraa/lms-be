using be.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "ADMIN")]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminUsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/admin/users/pending
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _context.Users
            .Where(u => u.ApprovalStatus == "PENDING")
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.EmployeeNumber,
                u.RegistrationSource,
                u.ApprovalStatus,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // PATCH: /api/admin/users/{id}/approve
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> ApproveUser(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        if (user.ApprovalStatus != "PENDING")
        {
            return BadRequest(new
            {
                message = "Only pending users can be approved."
            });
        }

        user.ApprovalStatus = "APPROVED";
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User approved successfully."
        });
    }

    // PATCH: /api/admin/users/{id}/reject
    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> RejectUser(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        if (user.ApprovalStatus != "PENDING")
        {
            return BadRequest(new
            {
                message = "Only pending users can be rejected."
            });
        }

        user.ApprovalStatus = "REJECTED";
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User rejected successfully."
        });
    }
}