using TaskManagementAPI.DTOs.Common;
using TaskManagementAPI.DTOs.Tasks;

namespace TaskManagementAPI.IServices
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTask(Guid UserId, string userRole, CreateTaskRequestDto dto);
        Task<IEnumerable<TaskResponseDto>> GetTasks(Guid userId, string role, TaskQueryParametersDto dto);
        Task<TaskResponseDto> GetTaskById(Guid userId, string role, Guid taskId);
        Task<TaskResponseDto> UpdateTask(Guid userId, string role, Guid taskId, UpdateTaskRequestDto dto);
        Task DeleteTask(Guid userId, string role, Guid taskId);
        Task<TaskResponseDto> UpdateStatus(Guid userId, string role, Guid taskId, UpdateTaskStatusRequestDto dto);

        Task<PagedResult<TaskResponseDto>> GetTasksPage(Guid userId, string role, TaskQueryParameters query);
    }
}
