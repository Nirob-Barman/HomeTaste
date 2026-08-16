using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Support
{
    public class SupportTicket : BaseEntity
    {
        public Guid UserId { get; private set; }  // Customer who raised the ticket
        public string? Subject { get; private set; }
        public string? Description { get; private set; }
        public TicketStatus Status { get; private set; }
        public DateTime? ResolvedAt { get; private set; }  // Nullable, will be set when the ticket is resolved
        public TicketPriority Priority { get; private set; }
        public string? MobileNo { get; private set; }
        public TimeSpan? ResolutionDuration { get; private set; }  // Duration to resolve the ticket


        // Department handling the ticket (Kitchen, Customer Support, Delivery, etc.)
        public Guid? DepartmentId { get; private set; }

        // Category of the issue (Food Quality, Delivery Issue, Order Problem, etc.)
        public Guid? CategoryTypeId { get; private set; }

        // Navigation property to User (if necessary, based on your models)
        // public ApplicationUser User { get; set; }

        private SupportTicket() { } // EF Core

        public static SupportTicket Create(
            Guid userId,
            string? subject,
            string? description,
            TicketPriority priority,
            string? mobileNo,
            Guid? departmentId,
            Guid? categoryTypeId)
        {
            return new SupportTicket
            {
                UserId = userId,
                Subject = subject,
                Description = description,
                Status = TicketStatus.Open,
                Priority = priority,
                MobileNo = mobileNo,
                DepartmentId = departmentId,
                CategoryTypeId = categoryTypeId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateStatus(TicketStatus status)
        {
            Status = status;

            if (status == TicketStatus.Resolved)
            {
                ResolvedAt = DateTime.UtcNow;
            }
        }
    }
}
