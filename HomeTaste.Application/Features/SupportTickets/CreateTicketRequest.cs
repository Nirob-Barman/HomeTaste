using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.SupportTickets
{
    public record CreateTicketRequest
    {
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; }
        public string? MobileNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CategoryTypeId { get; set; }
    }
}
