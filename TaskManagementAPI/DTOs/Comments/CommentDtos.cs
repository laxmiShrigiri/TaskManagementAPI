using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs.Comments;

public record CreateCommentDto(
    [Required, MaxLength(1000)] string content);

public record CommentResponseDto(
    Guid Id,
    string Content,
    DateTime CreatedAt,
    Guid UserId,
    string UserName,
    Guid TaskId);
