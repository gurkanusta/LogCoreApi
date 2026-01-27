using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc; 
using LogCoreApi.DTOs.Auth; 
using LogCoreApi.Entities; 
using LogCoreApi.Services.Auth; 
using LogCoreApi.Exceptions; 

namespace LogCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager; 
    private readonly SignInManager<AppUser> _signInManager; 
    private readonly RoleManager<IdentityRole> _roleManager; 
    private readonly TokenService _tokenService; 

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Email"] = new[] { "Email is required." },
                ["Password"] = new[] { "Password is required." }
            });
        }

        var user = new AppUser
        {
            UserName = dto.Email, 
            Email = dto.Email 
        };

        var result = await _userManager.CreateAsync(user, dto.Password); 

        if (!result.Succeeded) 
        {
            var errors = result.Errors.Select(e => e.Description).ToArray(); 
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Identity"] = errors
            });
        }

        
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        
        await _userManager.AddToRoleAsync(user, "User");

        
        var (token, expires) = await _tokenService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto { Token = token, ExpiresAtUtc = expires });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email); 

        if (user is null) 
            throw new NotFoundException("User not found"); 

        
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

        if (!result.Succeeded) 
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Credentials"] = new[] { "Invalid email or password." }
            });
        }

        
        var (token, expires) = await _tokenService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto { Token = token, ExpiresAtUtc = expires });
    }
}
