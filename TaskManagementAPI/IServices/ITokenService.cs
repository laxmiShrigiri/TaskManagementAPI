using TaskManagementAPI.Models;

namespace TaskManagementAPI.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
