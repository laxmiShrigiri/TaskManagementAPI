using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class AuthService(AppDbContext db, ITokenService tokenService) : IAuthService
    {
      public async Task<AuthResponseDto> Register(RegisterRequestDto dto)
      {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Registration details cannot be empty.");

            var exists = await db.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
            if(exists)
                throw new ConflictException($"A user already exists with email id {dto.Email}");
            var User = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                Role = dto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            db.Users.Add(User);
            await db.SaveChangesAsync();

            var token = tokenService.GenerateToken(User);

            return new AuthResponseDto(User.Id, User.Name, User.Email, User.Role.ToString(), token);
        }

        public async Task<AuthResponseDto> Login(LoginRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Login details cannot be empty.");

            var exists = await db.Users.FirstOrDefaultAsync(x=> x.Email== dto.Email.ToLower());

            if (exists == null || !BCrypt.Net.BCrypt.Verify(dto.Password, exists.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = tokenService.GenerateToken(exists);
            return new AuthResponseDto(exists.Id,exists.Name,exists.Email, exists.Role.ToString(),token);

        }

    }

   
}
