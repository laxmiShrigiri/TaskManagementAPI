using TaskManagementAPI.DTOs.Comments;

namespace TaskManagementAPI.IServices
{
    public interface ICommentService
    {
        Task<CommentResponseDto> AddComment(Guid userId, string role, Guid taskId, CreateCommentDto dto);
        Task<IEnumerable<CommentResponseDto>> GetCommentsByTaskId(Guid userId, string role, Guid taskId);
        Task DeleteComment(Guid userId, string role, Guid commentId);

    }
}
