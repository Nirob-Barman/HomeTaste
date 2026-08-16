using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketById
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<SupportTicketResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetTicketByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<SupportTicketResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FindAsync(new object?[] { request.TicketId }, cancellationToken);
            if (ticket == null)
                throw new NotFoundException("Ticket not found");

            return Result<SupportTicketResponse>.Ok(SupportTicketMapper.ToResponse(ticket), "Ticket retrieved successfully");
        }
    }
}
