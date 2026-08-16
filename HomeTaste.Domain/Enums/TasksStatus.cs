namespace HomeTaste.Domain.Enums
{
    public enum TasksStatus
    {
        Pending = 1,    // Task is pending and not yet completed
        InProgress = 2, // Task is in progress
        Completed = 3,  // Task has been completed
        Cancelled = 4   // Task was cancelled
    }
}
