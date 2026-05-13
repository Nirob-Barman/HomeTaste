using HomeTaste.Domain.Entities.Tasks;

namespace HomeTaste.Application.DTOs.TaskManagement
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
