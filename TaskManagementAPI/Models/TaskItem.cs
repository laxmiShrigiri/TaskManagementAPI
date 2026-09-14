namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid? AssignedToUserId { get; set; }
        public User? AssignedToUser {  get; set; }

        public Guid AssignedByUserId { get; set; }
       // public User? AssignedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set;  }
        public ICollection<Comment> Comments { get; set; } = [];
    }
}

