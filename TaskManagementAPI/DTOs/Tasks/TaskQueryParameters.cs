using TaskManagementAPI.DTOs.Common;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DTOs.Tasks
{
    public class TaskQueryParameters : PaginationParams
    {
        //Search
        public string? SearchTerm { get; set; }

        //Filters
        public TaskItemStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? ProjectId { get; set; }
        public DateTime? DueBefore { get; set; }

        //sorting
        public string? SortBy { get; set; } = "createdAt";
        public bool SortDescending { get; set; } = true;


    }
}
