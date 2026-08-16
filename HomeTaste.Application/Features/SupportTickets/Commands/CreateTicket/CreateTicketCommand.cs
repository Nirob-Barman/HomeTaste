using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Commands.CreateTicket
{
    public record CreateTicketCommand(
        string? Subject,
        string? Description,
        TicketPriority Priority,
        string? MobileNo,
        Guid? DepartmentId,
        Guid? CategoryTypeId) : IRequest<Result<Guid>>;
}
