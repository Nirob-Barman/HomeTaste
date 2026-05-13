
using HomeTaste.Domain.Entities.Support;

namespace HomeTaste.Application.DTOs.Support
{
    public class CreateTicketRequest
    {
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; }
        public string? MobileNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CategoryTypeId { get; set; }
    }
}
