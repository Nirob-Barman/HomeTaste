using HomeTaste.Domain.Entities.Support;

namespace HomeTaste.Application.DTOs.Support
{
    public class SupportTicketResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public string? MobileNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CategoryTypeId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
