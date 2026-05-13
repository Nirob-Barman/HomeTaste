using HomeTaste.Application.DTOs.TaskManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.TaskManagement;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Tasks;

namespace HomeTaste.Application.Services.TaskManagement
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tasks> _taskRepository;

        public TaskService(IUnitOfWork unitOfWork, IRepository<Tasks> taskRepository)
        {
            _unitOfWork = unitOfWork;
            _taskRepository = taskRepository;
        }
       
        public async Task<Result<PaginatedResponse<IEnumerable<TaskResponse>>>> GetAllTasksAsync(int pageNumber = 1, 
            int pageSize = 10, 
            string searchTerm = null!)
        {
            var query = _taskRepository.GetAllAsQueryable();
            //var totalCount = await _taskRepository.CountAsync(query);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(task =>
                    task.Title!.Contains(searchTerm) ||
                    task.Description!.Contains(searchTerm)
                );
            }
            var totalCount = await _taskRepository.CountAsync(query);
            var paginatedQuery = _taskRepository.PaginateAsQueryable(query, pageNumber, pageSize);
            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            var tasks = await _taskRepository.ToListAsync(paginatedQuery,
                task => new TaskResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status
                });

            var currentPageCount = tasks.Count();
            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<TaskResponse>>
            {
                MetaData = paginationMeta,
                Data = tasks,
            };

            return Result<PaginatedResponse<IEnumerable<TaskResponse>>>.Ok(response, "Tasks retrieved successfully", ResultType.Success);
        }

        public async Task<Result<int>> BulkInsertPredefinedTasksAsync()
        {
            try
            {
                var tasks = new List<Tasks>
                {
                    new Tasks { Title = "Make Pasta", Description = "Boil water, cook pasta, prepare sauce.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.High, Status = TasksStatus.Pending },
                    new Tasks { Title = "Bake Cake", Description = "Prepare batter, preheat oven, bake the cake.", DueDate = DateTime.Now.AddHours(4), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Salad", Description = "Chop vegetables, mix salad, add dressing.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Pizza", Description = "Prepare dough, add toppings, bake pizza.", DueDate = DateTime.Now.AddDays(1), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Smoothie", Description = "Blend fruits, add milk/yogurt, serve.", DueDate = DateTime.Now.AddHours(0.5), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Cook Soup", Description = "Prepare broth, chop vegetables, cook soup.", DueDate = DateTime.Now.AddHours(3), Priority = TaskPriority.High, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Sandwich", Description = "Cut bread, add fillings, pack sandwich.", DueDate = DateTime.Now.AddHours(0.5), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Grill Vegetables", Description = "Season vegetables, grill until tender.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.High, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Sandwiches", Description = "Toast bread, add fillings like lettuce, cheese, and meats.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Cook Rice", Description = "Boil water, add rice, cook until done.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Stir Fry", Description = "Chop vegetables, stir-fry with soy sauce and spices.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Guacamole", Description = "Mash avocados, add lime, onions, and tomatoes.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Pancakes", Description = "Mix ingredients, pour batter on pan, cook pancakes.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Smoothie Bowl", Description = "Blend fruits, serve in a bowl, top with granola and seeds.", DueDate = DateTime.Now.AddHours(0.5), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Bake Muffins", Description = "Mix batter, spoon into muffin tin, bake.", DueDate = DateTime.Now.AddHours(3), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Ice Cream", Description = "Mix ingredients, freeze, and churn.", DueDate = DateTime.Now.AddDays(1), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Roast Chicken", Description = "Season chicken, roast in oven until fully cooked.", DueDate = DateTime.Now.AddHours(5), Priority = TaskPriority.High, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Sushi", Description = "Prepare rice, cut fish, roll sushi.", DueDate = DateTime.Now.AddDays(1), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Soup Broth", Description = "Boil bones, vegetables, and herbs to make broth.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.High, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Omelette", Description = "Whisk eggs, pour into pan, cook until set, fold in fillings.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Tacos", Description = "Cook meat, chop veggies, assemble tacos.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.Medium, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Quinoa", Description = "Boil water, add quinoa, cook until fluffy.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Make Smoothie Popsicles", Description = "Blend fruits, pour into molds, freeze.", DueDate = DateTime.Now.AddHours(4), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Prepare Fruit Salad", Description = "Chop fruits, mix, and serve.", DueDate = DateTime.Now.AddHours(1), Priority = TaskPriority.Low, Status = TasksStatus.Pending },
                    new Tasks { Title = "Cook Mashed Potatoes", Description = "Boil potatoes, mash, add butter and cream.", DueDate = DateTime.Now.AddHours(2), Priority = TaskPriority.Medium, Status = TasksStatus.Pending }
                };

                var newTasks = new List<Tasks>();

                foreach (var task in tasks)
                {
                    var taskExists = await _taskRepository.AnyAsync(t => t.Title == task.Title);

                    if (!taskExists)
                    {
                        newTasks.Add(task);
                    }
                }

                if (!newTasks.Any())
                {
                    return Result<int>.Fail("All tasks already exist.", "No new tasks to insert", ResultType.Conflict);
                }

                await _taskRepository.AddRangeAsync(newTasks);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newTasks.Count, "New tasks related to food successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting tasks: {ex.Message}", "", ResultType.Failure);
            }
        }


        public async Task<Result<TaskResponse>> GetTaskByIdAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return Result<TaskResponse>.Fail("Task not found", "Task not found", ResultType.NotFound);

            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status
            };

            return Result<TaskResponse>.Ok(taskResponse, "Task retrieved successfully", ResultType.Success);
        }

        public async Task<Result<TaskResponse>> CreateTaskAsync(TaskRequest taskRequest)
        {
            var task = new Tasks
            {
                Title = taskRequest.Title,
                Description = taskRequest.Description,
                DueDate = taskRequest.DueDate,
                Priority = taskRequest.Priority,
                Status = taskRequest.Status
            };

            await _taskRepository.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status
            };

            return Result<TaskResponse>.Ok(taskResponse, "Task created successfully", ResultType.Success);
        }

        public async Task<Result<TaskResponse>> UpdateTaskAsync(Guid id, TaskRequest taskRequest)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return Result<TaskResponse>.Fail("Task not found", "Task not found", ResultType.NotFound);

            task.Title = taskRequest.Title ?? task.Title;
            task.Description = taskRequest.Description ?? task.Description;
            task.DueDate = taskRequest.DueDate;
            task.Priority = taskRequest.Priority;
            task.Status = taskRequest.Status;

            _taskRepository.Update(task);
            await _unitOfWork.SaveChangesAsync();

            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status
            };

            return Result<TaskResponse>.Ok(taskResponse, "Task updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteTaskAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return Result<bool>.Fail("Task not found", "Task not found", ResultType.NotFound);

            _taskRepository.Remove(task);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Task deleted successfully", ResultType.Success);
        }
    }
}
