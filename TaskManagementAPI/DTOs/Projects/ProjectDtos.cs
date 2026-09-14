using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs.Projects;

public record CreateProjectDto(
    [Required,MaxLength(100)] string Name,
    [Required,MaxLength(500)] string Description);

public record UpdateProjectDto(
     [Required] Guid ProjectId,
     [Required, MaxLength(100)] string Name,
     [Required, MaxLength(500)] string Description);

public record AddMemberRequestDto(
    [Required] Guid ProjectId,
    [Required] Guid userId);

public record MemberResponseDto(
    Guid userId,
    string Name,
    string Email,
    DateTime JoinedAt);

public record ProjectResponseDto(
    Guid Id,
    string Name,
    string Description,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    List<MemberResponseDto> Members);


