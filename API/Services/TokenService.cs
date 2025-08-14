using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot get token key in configuration");
        if (tokenKey.Length < 64)
            throw new Exception("Your token key needs to be >= 64 characters");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Country, "Singapore"),
            new("InvoiceNow","Peppol")
        };

        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credential
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(jwtToken);
    }
}
