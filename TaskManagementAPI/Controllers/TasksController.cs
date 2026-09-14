using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.DTOs.Tasks;
using TaskManagementAPI.IServices;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        private Guid userId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string role => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpPost("CreateTask")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDto dto) 
        {
            var result = await taskService.CreateTask(userId,role,dto);
            //return Ok(result);
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        }

        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskQueryParametersDto filter)
        {
            var result = await taskService.GetTasks(userId, role, filter);
            return Ok(result);
        }

        [HttpGet("GetTaskById/{id:guid}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await taskService.GetTaskById(userId, role, id);
            return Ok(result);
        }

        [HttpPut("UpdateTask/{id:guid}")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskRequestDto dto)
        {
            var result = await taskService.UpdateTask(userId, role,id, dto);
            return Ok(result);
        }

        [HttpDelete("DeleteTask/{id:guid}")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> DeleteTask(Guid id) 
        {
            await taskService.DeleteTask(userId, role, id);
            return NoContent();
        }

        [HttpPatch("UpdateTaskStatus/{id:guid}")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id,UpdateTaskStatusRequestDto dto)
        {
            var result = await taskService.UpdateStatus(userId, role, id, dto);
            return Ok(result);
        }
    }
}
