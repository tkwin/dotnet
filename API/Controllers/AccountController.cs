using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await IsEmailExists(registerDto.Email)) return BadRequest("email taken");
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var loginuser = await context.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (loginuser == null) return NotFound("Email is invalid");

        using var hmac = new HMACSHA512(loginuser.PasswordSalt);

        var inputHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        if (!inputHash.SequenceEqual(loginuser.PasswordHash))
            return BadRequest("Password is invalid");
        
       return loginuser.ToDto(tokenService);
    }

    private async Task<bool> IsEmailExists(string email)
    {
        return await context.Users.AnyAsync(x => EF.Functions.Like(x.Email, email));
    }
}
