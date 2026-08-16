using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Tasks
{
    public class Tasks : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TasksStatus Status { get; set; }
    }
}
