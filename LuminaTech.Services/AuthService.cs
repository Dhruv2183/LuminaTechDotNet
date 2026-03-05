using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LuminaTech.Data;
using LuminaTech.Data.Entities;

namespace LuminaTech.Services;

public interface IAuthService
{
    string GenerateJwtToken(User user, bool isDemo = false);
    Task<User> FindOrCreateUser(string googleId, string email, string fullName, string profilePictureUrl);
}

public class AuthService : IAuthService
{
    private readonly LuminaTechDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(LuminaTechDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user, bool isDemo = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "LuminaTech-Super-Secret-Key-2024-Change-In-Production-Please!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("googleId", user.GoogleId),
            new Claim("profilePicture", user.ProfilePictureUrl)
        };

        if (isDemo)
            claims.Add(new Claim("isDemo", "true"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "LuminaTech",
            audience: _configuration["Jwt:Audience"] ?? "LuminaTech",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User> FindOrCreateUser(string googleId, string email, string fullName, string profilePictureUrl)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        if (user == null)
        {
            user = new User
            {
                GoogleId = googleId,
                Email = email,
                FullName = fullName,
                ProfilePictureUrl = profilePictureUrl,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update profile info on each login
            user.FullName = fullName;
            user.ProfilePictureUrl = profilePictureUrl;
            await _context.SaveChangesAsync();
        }

        return user;
    }
}
