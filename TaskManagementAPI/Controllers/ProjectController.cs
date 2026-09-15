using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.DTOs.Projects;
using TaskManagementAPI.IServices;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string Role => User.FindFirstValue(ClaimTypes.Role)!;
        
        

        [HttpPost("CreateProject")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var result = await projectService.CreateProject(UserId, dto);
            return CreatedAtAction(nameof(GetProjectById),new { id = result.Id },result);
        }

        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            var result = await projectService.GetUserProjectsAsync(UserId, Role);
            return Ok(result);
        }

        [HttpGet("GetProjectById/{id:guid}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var result = await projectService.GetProjectById(UserId,Role, id);
            return Ok(result);
        }

        [HttpPut("UpdateProject")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectDto dto)
        {
            var result = await projectService.UpdateProject(UserId, Role, dto);
            return Ok(result);
        }

        [HttpDelete("DeleteProject/{id:guid}")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            await projectService.DeleteProject(id, UserId, Role);
            return NoContent();
        }

        [HttpPost("AddMember")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AddMember([FromBody]AddMemberRequestDto dto)
        {
            await projectService.AddMember(UserId, Role, dto);
            return Ok(new { message = "Member added successfully." });
        }


        [HttpDelete("RemoveMember")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> RemoveMember([FromBody] AddMemberRequestDto dto)
        {
            await projectService.RemoveMember(UserId, Role, dto);
            return NoContent();
        }

        [HttpGet("GetMembers/{id:guid}")]
        public async Task<IActionResult> GetMembers(Guid id)
        {
            var result = await projectService.GetProjectMembers(UserId, id, Role);
            return Ok(result);
        }
    }
}
