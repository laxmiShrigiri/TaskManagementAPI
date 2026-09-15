using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.Comments;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class CommentService(AppDbContext db) : ICommentService
    {
        public async Task<CommentResponseDto> AddComment(Guid userId, string role, Guid taskId, CreateCommentDto dto)
        {
            var task = await db.Tasks.Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == taskId);

            if (task is null)
                throw new KeyNotFoundException("Task not found.");

            await EnsureCanAccessProject(userId,role, task.ProjectId);

            var comment = new Comment
            {
                Content = dto.content,
                TaskId = taskId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };
            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);
            return new CommentResponseDto(
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.UserId,
                user?.Name ?? string.Empty,
                comment.TaskId);

        }

        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByTaskId(Guid userId, string role, Guid taskId)
        {
            var task = await db.Tasks.Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == taskId);

            if (task is null)
                throw new KeyNotFoundException("Task not found.");

            await EnsureCanAccessProject(userId, role, task.ProjectId);
            var comments = await db.Comments
                .Where(m=> m.TaskId== taskId)
                .Include(m=> m.User)
                .OrderByDescending(m=>m.CreatedAt)
                .ToListAsync();
            return comments.Select(m => new CommentResponseDto(
                m.Id,
                m.Content,
                m.CreatedAt,
                m.UserId,
                m.User?.Name ?? string.Empty,
                m.TaskId));
        }

        public async Task DeleteComment(Guid userId, string role, Guid commentId)
        {
            var comment = await db.Comments
                .Include(t=> t.Task)
                .ThenInclude(p=>p.Project)
                .FirstOrDefaultAsync(x=> x.Id == commentId);
            if(comment is null)
                throw new KeyNotFoundException("Comment not found.");

            var isAuthor = comment.UserId == userId;
            var isProjectOwner = role == nameof(UserRole.ProjectManager) && comment.Task.Project.CreatedByUserId == userId;
            var isAdmin = role == nameof(UserRole.Admin);
            if (!isAuthor && !isProjectOwner && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to delete this comment.");
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
        }


        private async Task EnsureCanAccessProject(Guid userId, string role, Guid projectId)
        {
            if (role == nameof(UserRole.Admin)) return;

            var isMember = await db.ProjectMembers.AnyAsync(m=> m.ProjectId== projectId && m.userId == userId);
            if (!isMember)
                throw new UnauthorizedAccessException("You are not authorized to access this project's tasks.");
        }
    }

}
