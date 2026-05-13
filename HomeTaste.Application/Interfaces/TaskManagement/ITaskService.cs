using HomeTaste.Application.DTOs.TaskManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.TaskManagement
{
    public interface ITaskService
    {
        Task<Result<PaginatedResponse<IEnumerable<TaskResponse>>>> GetAllTasksAsync(int pageNumber = 1,
            int pageSize = 10, 
            string searchTerm = null!);
        Task<Result<TaskResponse>> GetTaskByIdAsync(Guid id);
        Task<Result<TaskResponse>> CreateTaskAsync(TaskRequest taskRequest);
        Task<Result<int>> BulkInsertPredefinedTasksAsync();
        Task<Result<TaskResponse>> UpdateTaskAsync(Guid id, TaskRequest taskRequest);
        Task<Result<bool>> DeleteTaskAsync(Guid id);
    }
}
