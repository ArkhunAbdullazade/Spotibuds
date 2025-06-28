using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AuthController(
        UserManager<User> userManager, 
        SignInManager<User> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            IsPrivate = dto.IsPrivate ?? false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { message = "User registered successfully", userId = user.Id });
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Username, dto.Password, dto.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            var roles = await _userManager.GetRolesAsync(user!);

            return Ok(new 
            { 
                message = "Login successful",
                userId = user!.Id,
                username = user.UserName,
                email = user.Email,
                roles = roles
            });
        }

        return BadRequest("Invalid username or password");
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Get user profile
    /// </summary>
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            id = user.Id,
            username = user.UserName,
            email = user.Email,
            isPrivate = user.IsPrivate,
            createdAt = user.CreatedAt,
            roles = roles
        });
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        user.IsPrivate = dto.IsPrivate ?? user.IsPrivate;
        
        if (!string.IsNullOrEmpty(dto.Email))
        {
            user.Email = dto.Email;
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "User updated successfully" });
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    [HttpPost("users/{id}/roles/{role}")]
    public async Task<IActionResult> AssignRole(Guid id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return BadRequest("Role does not exist");
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            return BadRequest("User already has this role");
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        if (result.Succeeded)
        {
            return Ok(new { message = $"Role '{role}' assigned successfully" });
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Search users by username
    /// </summary>
    [HttpGet("users/search")]
    public IActionResult SearchUsers([FromQuery] string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username parameter is required");
        }

        var users = _userManager.Users
            .Where(u => u.UserName!.Contains(username))
            .Take(10)
            .Select(u => new
            {
                id = u.Id,
                username = u.UserName,
                isPrivate = u.IsPrivate
            })
            .ToList();

        return Ok(users);
    }
}

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool? IsPrivate { get; set; }
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public bool? IsPrivate { get; set; }
} 