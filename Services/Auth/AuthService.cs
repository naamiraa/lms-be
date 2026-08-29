using Microsoft.AspNetCore.Identity;
using be.Data;
using be.DTOs.Auth;
using be.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace be.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            throw new Exception("Email is already registered.");
        }

        var participantRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "PARTICIPANT");

        if (participantRole == null)
        {
            throw new Exception("Participant role is not configured.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            EmployeeNumber = request.EmployeeNumber,
            RegistrationSource = "SELF_REGISTERED",
            ApprovalStatus = "PENDING",
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        user.UserRoles.Add(new UserRole
        {
            User = user,
            Role = participantRole
        });

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "Registration successful. Waiting for admin approval.";
    }



    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            throw new Exception("Email or password is incorrect.");
        }

        if (user.PasswordHash == null)
        {
            throw new Exception("This account does not use password authentication.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new Exception("Email or password is incorrect.");
        }

        if (user.ApprovalStatus != "APPROVED")
        {
            throw new Exception("Your account is waiting for administrator approval.");
        }

        if (!user.IsActive)
        {
            throw new Exception("Your account is inactive.");
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role.Name)
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]
                ?? throw new Exception("JWT key is not configured.")
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var expiryMinutes = int.Parse(
            _configuration["Jwt:ExpiryMinutes"] ?? "120"
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = roles
        };
    }
}