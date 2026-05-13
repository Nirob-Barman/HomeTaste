
namespace HomeTaste.Domain.Entities.Support
{
    public class SupportTicket : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }  // Customer who raised the ticket
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime? ResolvedAt { get; set; }  // Nullable, will be set when the ticket is resolved
        public TicketPriority Priority { get; set; }
        public string? MobileNo { get; set; }
        public TimeSpan? ResolutionDuration { get; set; }  // Duration to resolve the ticket


        // Department handling the ticket (Kitchen, Customer Support, Delivery, etc.)
        public Guid? DepartmentId { get; set; }

        // Category of the issue (Food Quality, Delivery Issue, Order Problem, etc.)
        public Guid? CategoryTypeId { get; set; }

        // Navigation property to User (if necessary, based on your models)
        // public ApplicationUser User { get; set; }
    }
   
    public enum TicketStatus
    {
        Open = 1,
        Resolved = 2,
        InProgress = 3,
        Closed = 4
    }

    public enum TicketPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }


}
