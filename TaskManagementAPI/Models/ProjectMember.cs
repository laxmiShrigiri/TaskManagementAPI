namespace TaskManagementAPI.Models
{
    public class ProjectMember
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid userId {  get; set; }
        public User user { get; set; } = null!;
        public DateTime JoinedAt {  get; set; } = DateTime.UtcNow;
    }
}
