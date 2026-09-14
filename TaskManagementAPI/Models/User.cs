namespace TaskManagementAPI.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }  = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Member;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProjectMember> ProjectMemberships { get; set; } = [];
        public ICollection<TaskItem> AssignedTask { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
