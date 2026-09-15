using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.Projects;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class ProjectService(AppDbContext db) : IProjectService
    {
        public async Task<ProjectResponseDto> CreateProject(Guid currentUserId, CreateProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = currentUserId
            };

            var member = new ProjectMember
            {
                ProjectId = project.Id,
                userId = currentUserId,
                JoinedAt = DateTime.UtcNow
            };
            project.Members.Add(member);

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            return await GetProjectById(currentUserId, nameof(UserRole.Admin), project.Id);
            //  return MapToResponseDto(project);
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(Guid currentUserId, string role)
        {
            IQueryable<Project> query = db.Projects
                .Include(p => p.Members)
                .ThenInclude(m => m.user);

            // Members and PMs only see projects they belong to (Admins see all)
            if (role != nameof(UserRole.Admin))
            {
                query = query.Where(p => p.Members.Any(m => m.userId == currentUserId));
            }

            var projects = await query.ToListAsync();

            return projects.Select(MapToResponseDto);
        }

        public async Task<ProjectResponseDto> GetProjectById(Guid userId, string role, Guid projectId)
        {
            var project = await db.Projects
                .Include(m => m.Members)
                .ThenInclude(u => u.user)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
                throw new KeyNotFoundException("Project not found.");

            if (role != nameof(UserRole.Admin) && !project.Members.Any(m => m.userId == userId))
                throw new UnauthorizedAccessException("You are not authorized to view this project.");
            return MapToResponseDto(project);
        }

        public async Task<ProjectResponseDto> UpdateProject(Guid userId, string role, UpdateProjectDto dto)
        {
            var project = await db.Projects
               .Include(m => m.Members)
               .ThenInclude(u => u.user)
               .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if (project is null)
                throw new KeyNotFoundException("Project not found.");
            EnsureCanManageProject(project, userId, role);

            project.Name = dto.Name;
            project.Description = dto.Description;
            await db.SaveChangesAsync();
            return MapToResponseDto(project);

        }

        public async Task DeleteProject(Guid projecId, Guid userId, string role)
        {
            var project = await db.Projects.FirstOrDefaultAsync(x => x.Id == projecId);
            if (project is null)
                throw new KeyNotFoundException("Project not found.");
            EnsureCanManageProject(project, userId, role);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
        }

        public async Task AddMember(Guid userId, string role, AddMemberRequestDto dto)
        {
            var project = await db.Projects
                .Include(m => m.Members)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if(project is null)
                throw new KeyNotFoundException("Project not found.");
            EnsureCanManageProject(project, userId, role);

            var userExist = await db.Users.AnyAsync(x => x.Id == dto.userId);
            if(!userExist)
                throw new KeyNotFoundException("Target user not found.");

            var alreadyMember = project.Members.Any(x => x.userId ==dto.userId);

            if(alreadyMember)
                throw new ConflictException("User is already a member of this project.");

            project.Members.Add(new ProjectMember
            {
                ProjectId = dto.ProjectId,
                userId = dto.userId,
                JoinedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        public async Task RemoveMember(Guid userId, string role, AddMemberRequestDto dto)
        {
            var project = await db.Projects
                .Include(m => m.Members)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if (project is null)
                throw new KeyNotFoundException("Project not found.");
            EnsureCanManageProject(project, userId, role);

            var userExist = await db.Users.AnyAsync(x => x.Id == dto.userId);
            if (!userExist)
                throw new KeyNotFoundException("Target user not found.");

            var member = project.Members.FirstOrDefault(x => x.userId == dto.userId);
            if(member is null)
                throw new KeyNotFoundException("User is not a member of this project.");
            project.Members.Remove(member);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<MemberResponseDto>> GetProjectMembers(Guid userId, Guid projectId, string role)
        {
            var project = await db.Projects
                .Include(m => m.Members)
                .ThenInclude(u => u.user)
                .FirstOrDefaultAsync(x => x.Id == projectId);
            if (project is null)
                throw new KeyNotFoundException("Project not found.");

            if(role!= nameof(UserRole.Admin) && !project.Members.Any(m=> m.userId==userId))
                throw new UnauthorizedAccessException("You are not authorized to view members of this project.");

            return project.Members.Select(m =>new MemberResponseDto
            (
                m.userId,
            m.user.Name,
            m.user.Email,
            m.JoinedAt
            ));

        }
        private ProjectResponseDto MapToResponseDto(Project project)
        {
            return new ProjectResponseDto(
                project.Id,
                project.Name,
                project.Description,
                project.CreatedByUserId,
                project.CreatedAt,
                project.Members.Select(m => new MemberResponseDto(
                    m.userId,
                    m.user.Name,
                    m.user.Email,
                    m.JoinedAt
                )).ToList()
            );
        }

        private void EnsureCanManageProject(Project project, Guid userid, string role)
        {
            if (role == nameof(UserRole.Admin)) return;

            if(role  == nameof(UserRole.ProjectManager) && project.CreatedByUserId == userid) return;
            throw new UnauthorizedAccessException("You do not have permission to modify this project.");

        }
    }
}
