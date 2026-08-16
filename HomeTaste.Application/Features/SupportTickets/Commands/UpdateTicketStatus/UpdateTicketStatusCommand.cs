using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Commands.UpdateTicketStatus
{
    public record UpdateTicketStatusCommand(Guid TicketId, TicketStatus Status) : IRequest<Result<string>>;
}
