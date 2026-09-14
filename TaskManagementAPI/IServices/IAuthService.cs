using TaskManagementAPI.DTOs.Auth;

namespace TaskManagementAPI.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterRequestDto dto);
        Task<AuthResponseDto> Login(LoginRequestDto dto);
    }
}
