using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using TaskManagementAPI.DTOs.Comments;
using TaskManagementAPI.IServices;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController(ICommentService commentService) : ControllerBase
    {
        private Guid userId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string userRole => User.FindFirstValue(ClaimTypes.Role)!;
        [HttpPost("AddComment/{id:guid}")]
        public async Task<IActionResult> AddComment(Guid id, CreateCommentDto dto)
        {
            var result = await commentService.AddComment(userId, userRole, id, dto);
            return Ok(result);
        }

        [HttpGet("GetComment/{id:guid}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var result = await commentService.GetCommentsByTaskId(userId, userRole, id);
            return Ok(result);
        }

        [HttpDelete("DeleteComment/{id:guid}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            await commentService.DeleteComment(userId, userRole, id);
            return NoContent();
        }
    }
}
