namespace HomeTaste.Application.Features.SupportTickets
{
    public static class SupportTicketMapper
    {
        public static SupportTicketResponse ToResponse(HomeTaste.Domain.Entities.Support.SupportTicket ticket) => new()
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Status = ticket.Status,
            Priority = ticket.Priority,
            MobileNo = ticket.MobileNo,
            DepartmentId = ticket.DepartmentId,
            CategoryTypeId = ticket.CategoryTypeId,
            CreatedAt = ticket.CreatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}
