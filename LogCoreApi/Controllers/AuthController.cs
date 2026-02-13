using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LogCoreApi.DTOs.Auth;
using LogCoreApi.Entities;
using LogCoreApi.Services.Auth;
using LogCoreApi.Exceptions;
using LogCoreApi.Models;

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

    public record RegisterRequest(string Email, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var existing = await _userManager.FindByEmailAsync(req.Email);
        if (existing is not null)
            return BadRequest(ApiResponse<object>.Fail("AUTH-400", "Email already in use.", HttpContext));

        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            var details = result.Errors.Select(e => new { e.Code, e.Description }).ToArray();
            return BadRequest(ApiResponse<object>.Fail("AUTH-400", "Register failed.", HttpContext, details));
        }

        const string defaultRole = "User";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
            await _roleManager.CreateAsync(new IdentityRole(defaultRole));

        await _userManager.AddToRoleAsync(user, defaultRole);

        return Ok(ApiResponse<object>.Ok(new { message = "Registered successfully." }, HttpContext));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return NotFound(ApiResponse<object>.Fail("AUTH-404", "User not found.", HttpContext));

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
            return Unauthorized(ApiResponse<object>.Fail("AUTH-401", "Invalid email or password.", HttpContext));

        var (token, expiresAtUtc) = await _tokenService.CreateTokenAsync(user);

        return Ok(ApiResponse<object>.Ok(new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        }, HttpContext));
    }
}