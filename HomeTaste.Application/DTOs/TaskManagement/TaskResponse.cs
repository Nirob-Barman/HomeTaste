using HomeTaste.Domain.Entities.Tasks;

namespace HomeTaste.Application.DTOs.TaskManagement
{
    public class TaskResponse
    {
        public Guid Id { get; set; }               
        public string? Title { get; set; }         
        public string? Description { get; set; }   
        public DateTime DueDate { get; set; }     
        public TaskPriority Priority { get; set; } 
        public TasksStatus Status { get; set; }  
    }
}
