using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Support
{
    public interface ISupportTicketService
    {
        Task<Result<Guid>> CreateTicketAsync(CreateTicketRequest request);
        Task<Result<SupportTicketResponse>> GetTicketByIdAsync(Guid ticketId);
        Task<Result<string>> UpdateTicketStatusAsync(Guid ticketId, UpdateTicketRequest request);
        Task<Result<IEnumerable<SupportTicketResponse>>> GetTicketsByUserIdAsync(Guid userId);
        Task<Result<IEnumerable<SupportTicketResponse>>> GetAllTicketsAsync();
    }
}
