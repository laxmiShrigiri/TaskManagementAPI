using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.Common;
using TaskManagementAPI.DTOs.Tasks;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Models;
using static System.Net.WebRequestMethods;

namespace TaskManagementAPI.Services
{
    public class TaskService(AppDbContext db) : ITaskService
    {

        public async Task<TaskResponseDto> CreateTask(Guid UserId, string userRole, CreateTaskRequestDto dto)
        {
            var project = await db.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

            if (project is null)
                throw new KeyNotFoundException("Project not found.");

            EnsureCanManageProject(project, UserId, userRole);

            if (dto.AssignedToUserId.HasValue)
            {
                var isMember = project.Members.Any(m=> m.userId== dto.AssignedToUserId.Value);
                if(!isMember)
                    throw new InvalidOperationException("Assigned user is not a member of this project.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = TaskItemStatus.Todo,
                DueDate = dto.DueDate,
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                AssignedByUserId = UserId,
                CreatedAt = DateTime.UtcNow
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            return await GetTaskById(UserId, userRole,task.Id );
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasks(Guid userId, string role, TaskQueryParametersDto dto)
        {
            IQueryable<TaskItem> query = db.Tasks
                .Include(p => p.Project)
                .Include(t => t.AssignedToUser);
            if (role != nameof(UserRole.Admin))
                query = query.Where(u => db.ProjectMembers.Any(pm => pm.ProjectId == u.ProjectId && pm.userId == userId));

            if(dto.ProjectId.HasValue)
                query = query.Where(t=> t.ProjectId == dto.ProjectId.Value);

            if(dto.Status.HasValue)
                query = query.Where(s=> s.Status == dto.Status.Value);

            if(dto.Priority.HasValue)
                query = query.Where(p=> p.Priority  == dto.Priority.Value);

            if(dto.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == dto.AssignedToUserId.Value);

            if(!string.IsNullOrWhiteSpace(dto.SearchItem))
            {
                var search = dto.SearchItem.Trim().ToLower();
                query = query.Where(t=> t.Title.ToLower().Contains(search) || t.Description.ToLower().Contains(search));
            }
            var task = await query.ToListAsync();
            return task.Select(MapToResponseDto);
        }

        public async Task<TaskResponseDto> GetTaskById(Guid userId, string role, Guid taskId)
        {
            var task = await db.Tasks
                .Include(m => m.Project)
                .Include(m => m.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task is null)
                throw new KeyNotFoundException("Task not found.");

            if (role != nameof(UserRole.Admin))
            {

                var isMember = await db.ProjectMembers.AnyAsync(pm => pm.ProjectId == task.ProjectId && pm.userId == userId);
                if (!isMember)
                    throw new UnauthorizedAccessException("You are not authorized to view this task.");
            }
           
            return MapToResponseDto(task);
        }

        public async Task<TaskResponseDto> UpdateTask(Guid userId, string role, Guid taskId, UpdateTaskRequestDto dto)
        {
            var task = await db.Tasks.Include(m => m.Project)
                .FirstOrDefaultAsync(t=>t.Id == taskId);
            if(task is null)
                throw new KeyNotFoundException("Task not found.");

            EnsureCanManageProject(task.Project, userId, role);
            if (dto.AssignedToUserId.HasValue)
            {
                var isMember = await db.ProjectMembers.AnyAsync(m=> m.userId == dto.AssignedToUserId.Value && m.ProjectId == task.ProjectId);
                if(!isMember)
                    throw new InvalidOperationException("Assigned user must be a member of this project.");
            }
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.AssignedToUserId = dto.AssignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return await GetTaskById(userId, role, task.Id);
        }

        public async Task DeleteTask(Guid userId, string role, Guid taskId)
        {
            var task = await db.Tasks.Include(m => m.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if(task is null)
                throw new KeyNotFoundException("Task not found.");

            EnsureCanManageProject(task.Project, userId,role);

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
        }

        public async Task<TaskResponseDto> UpdateStatus(Guid userId, string role, Guid taskId, UpdateTaskStatusRequestDto dto)
        {
            var task = await db.Tasks.Include(m=>m.Project)
                .FirstOrDefaultAsync(m=>m.Id ==  taskId);
            if (task is null)
                throw new KeyNotFoundException("Task not found.");

            if(role == nameof(UserRole.Member) && task.AssignedToUserId!=userId)
                throw new UnauthorizedAccessException("You can only change the status of tasks assigned to you.");

            if(task.Status== TaskItemStatus.Cancelled && dto.Status== TaskItemStatus.Completed)
                throw new InvalidOperationException("A cancelled task cannot be marked directly as completed.");
            task.Status = dto.Status;
            task.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return await GetTaskById(userId, role, taskId);

        }

        public async Task<PagedResult<TaskResponseDto>> GetTasksPage(Guid userId, string role, TaskQueryParameters query)
        {
            var taskQuery = db.Tasks
                .Include(m => m.Project)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if(role != nameof(UserRole.Admin))
            {
                var accessibleProjectIds = db.ProjectMembers
                    .Where(pm => pm.userId == userId)
                    .Select(m => m.ProjectId);

                taskQuery = taskQuery.Where(t => accessibleProjectIds.Contains(t.ProjectId));
            }

            if(query.ProjectId.HasValue)
                taskQuery = taskQuery.Where(t=>t.ProjectId == query.ProjectId.Value);

            if (query.Status.HasValue)
                taskQuery = taskQuery.Where(t => t.Status == query.Status.Value);

            if (query.Priority.HasValue)
                taskQuery = taskQuery.Where(t => t.Priority == query.Priority.Value);

            if (query.AssignedToUserId.HasValue)
                taskQuery = taskQuery.Where(t => t.AssignedToUserId == query.AssignedToUserId.Value);

            if (query.DueBefore.HasValue)
                taskQuery = taskQuery.Where(t => t.DueDate <= query.DueBefore.Value);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim().ToLower();
                taskQuery = taskQuery.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    (t.Description != null && t.Description.ToLower().Contains(term)));
            }

            // 4. Dynamic Sorting
            taskQuery = (query.SortBy?.ToLower()) switch
            {
                "duedate" => query.SortDescending ? taskQuery.OrderByDescending(t => t.DueDate) : taskQuery.OrderBy(t => t.DueDate),
                "priority" => query.SortDescending ? taskQuery.OrderByDescending(t => t.Priority) : taskQuery.OrderBy(t => t.Priority),
                "title" => query.SortDescending ? taskQuery.OrderByDescending(t => t.Title) : taskQuery.OrderBy(t => t.Title),
                _ => query.SortDescending ? taskQuery.OrderByDescending(t => t.CreatedAt) : taskQuery.OrderBy(t => t.CreatedAt)
            };
            var totalCount = await taskQuery.CountAsync();

            // 6. Pagination execution (.Skip and .Take)
            var items = await taskQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new TaskResponseDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.Priority.ToString(),
                    t.DueDate,
                    t.ProjectId,
                    t.Project.Name,
                    t.AssignedToUserId,
                    t.AssignedToUser != null ? t.AssignedToUser.Name : null,
                    t.AssignedByUserId,
                    t.CreatedAt,
                    t.UpdatedAt
                ))
                .ToListAsync();

            return new PagedResult<TaskResponseDto>(items, totalCount, query.PageNumber, query.PageSize);
        

        }



        private void EnsureCanManageProject(Project project, Guid currentUserId, string role)
        {
            if (role == nameof(UserRole.Admin)) return;
            if (role == nameof(UserRole.ProjectManager) && project.CreatedByUserId == currentUserId) return;

            throw new UnauthorizedAccessException("You do not have permission to manage tasks in this project.");
        }

        private TaskResponseDto MapToResponseDto(TaskItem task)
        {
            return new TaskResponseDto(
                task.Id,
                task.Title,
                task.Description,
                task.Status.ToString(),
                task.Priority.ToString(),
                task.DueDate,
                task.ProjectId,
                task.Project?.Name ?? string.Empty,
                task.AssignedToUserId,
                task.AssignedToUser?.Name,
                task.AssignedByUserId,
                task.CreatedAt,
                task.UpdatedAt
            );
        }
    }
}
