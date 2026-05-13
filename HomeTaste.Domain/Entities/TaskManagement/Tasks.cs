
namespace HomeTaste.Domain.Entities.Tasks
{
    public class Tasks : BaseEntity
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TasksStatus Status { get; set; }
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
    public enum TasksStatus
    {
        Pending = 1,    // Task is pending and not yet completed
        InProgress = 2, // Task is in progress
        Completed = 3,  // Task has been completed
        Cancelled = 4   // Task was cancelled
    }
}
