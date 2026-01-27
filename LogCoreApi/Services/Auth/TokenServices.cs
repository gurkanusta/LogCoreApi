using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.IdentityModel.Tokens; 
using System.Text; 
using LogCoreApi.Entities; 
namespace LogCoreApi.Services.Auth;

public class TokenService
{
    private readonly IConfiguration _config; 
    private readonly UserManager<AppUser> _userManager; 
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _config = config; 
        _userManager = userManager; 
    }

    public async Task<(string Token, DateTime ExpiresAtUtc)> CreateTokenAsync(AppUser user)
    {
        var key = _config["Jwt:Key"]!; 
        var issuer = _config["Jwt:Issuer"]!; 
        var audience = _config["Jwt:Audience"]!; 
        var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!); 

        var roles = await _userManager.GetRolesAsync(user); 

        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), 
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""), 
            new Claim(ClaimTypes.NameIdentifier, user.Id), 
            new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "") 
        };

        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role)); 
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); 
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256); 

        var expires = DateTime.UtcNow.AddMinutes(expireMinutes); 

        var token = new JwtSecurityToken(
            issuer: issuer, 
            audience: audience, 
            claims: claims, 
            expires: expires, 
            signingCredentials: creds 
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token); 

        return (tokenString, expires); 
    }
}
