using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Commands.UpdateTicketStatus
{
    public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTicketStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<string>> Handle(UpdateTicketStatusCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FindAsync(new object?[] { command.TicketId }, cancellationToken);
            if (ticket == null)
                throw new NotFoundException("Ticket not found");

            ticket.UpdateStatus(command.Request.Status);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok("Ticket status updated successfully", "Ticket status updated");
        }
    }
}
