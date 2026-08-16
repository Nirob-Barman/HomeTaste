using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Commands.CreateTicket
{
    public class CreateTicketCommand : IRequest<Result<Guid>>
    {
        public CreateTicketRequest Request { get; set; }

        public CreateTicketCommand(CreateTicketRequest request)
        {
            Request = request;
        }
    }
}
