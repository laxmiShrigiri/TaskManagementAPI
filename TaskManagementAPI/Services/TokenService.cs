using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class TokenService(IConfiguration config): ITokenService
    {
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier ,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim (ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
           // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: config["Jwt:Issuer"],
               audience: config["Jwt:Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddDays(10),
               signingCredentials: creds
               );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
