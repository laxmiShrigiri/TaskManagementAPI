namespace TaskManagementAPI.Models
{
    //public class Enum
    //{
        public enum UserRole
        {
            Admin,
            ProjectManager,
            Member
        };

        public enum TaskItemStatus{
            Todo,
            InProgress,
            Completed,
            Cancelled
        };

        public enum TaskPriority
        {
            Low,
            Medium,
            High,
            Critical
        };
    //}
}
