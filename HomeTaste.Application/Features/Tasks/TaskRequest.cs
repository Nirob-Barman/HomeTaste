using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Tasks
{
    public class TaskRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TasksStatus Status { get; set; }
    }
}
