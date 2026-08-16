using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using TicketEntity = HomeTaste.Domain.Entities.Support.SupportTicket;

namespace HomeTaste.Application.Features.SupportTickets.Commands.CreateTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateTicketCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<Guid>> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedException("Invalid User ID.");

            var ticket = TicketEntity.Create(
                userId,
                request.Subject,
                request.Description,
                request.Priority,
                request.MobileNo,
                request.DepartmentId,
                request.CategoryTypeId);

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Ok(ticket.Id, "Ticket created successfully");
        }
    }
}
