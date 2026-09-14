namespace TaskManagementAPI.DTOs.Auth;

public record AuthResponseDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Tokne
    );
    
