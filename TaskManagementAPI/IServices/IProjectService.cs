using TaskManagementAPI.DTOs.Projects;

namespace TaskManagementAPI.IServices
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateProject(Guid currentUserId, CreateProjectDto dto);
        Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(Guid currentUserId, string role);
        Task<ProjectResponseDto> GetProjectById(Guid userId, string role, Guid projectId);
        Task<ProjectResponseDto> UpdateProject(Guid userId, string role, UpdateProjectDto dto);
        Task DeleteProject(Guid projecId, Guid userId, string role);
        Task AddMember(Guid userId, string role, AddMemberRequestDto dto);
        Task RemoveMember(Guid userId, string role, AddMemberRequestDto dto);
        Task<IEnumerable<MemberResponseDto>> GetProjectMembers(Guid userId, Guid projectId, string role);


    }
}

