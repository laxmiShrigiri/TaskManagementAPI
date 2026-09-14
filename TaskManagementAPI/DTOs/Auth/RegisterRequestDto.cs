using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DTOs.Auth;

public record RegisterRequestDto
(
       [Required] string Name,
       [Required, EmailAddress] string Email,
       [Required, MinLength(6)] string Password,
       UserRole Role = UserRole.Member
);
     

    

