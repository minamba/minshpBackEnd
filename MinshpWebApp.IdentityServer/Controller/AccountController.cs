using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.IdentityServer.Authentication;

namespace MinshpWebApp.IdentityServer.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> um, SignInManager<AppUser> sm)
    {
        _userManager = um; _signInManager = sm;
    }

    public record RegisterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var u = new AppUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
        var res = await _userManager.CreateAsync(u, dto.Password);
        return res.Succeeded ? Ok(new { message = "registered" }) : BadRequest(res.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var u = await _userManager.FindByEmailAsync(dto.Email);
        if (u is null) return Unauthorized();
        var res = await _signInManager.PasswordSignInAsync(u, dto.Password, isPersistent: true, lockoutOnFailure: false);
        return res.Succeeded ? Ok(new { message = "logged-in" }) : Unauthorized();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}
