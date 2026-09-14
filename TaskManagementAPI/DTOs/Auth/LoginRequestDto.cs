using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs.Auth;

public record LoginRequestDto(
    [Required,EmailAddress] string Email,
    [Required] string Password
    );
   

