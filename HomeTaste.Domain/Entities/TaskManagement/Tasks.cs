using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Tasks
{
    public class Tasks : BaseEntity
    {
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TasksStatus Status { get; private set; }

        private Tasks() { } // EF Core

        public static Tasks Create(string? title, string? description, DateTime dueDate, TaskPriority priority, TasksStatus status)
        {
            return new Tasks
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                Status = status
            };
        }

        public void UpdateDetails(string? title, string? description, DateTime dueDate, TaskPriority priority, TasksStatus status)
        {
            Title = title ?? Title;
            Description = description ?? Description;
            DueDate = dueDate;
            Priority = priority;
            Status = status;
        }
    }
}
