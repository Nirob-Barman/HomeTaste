using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.Support;
using HomeTaste.Application.Validators.Support;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Support;

namespace HomeTaste.Application.Services.Support
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public SupportTicketService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<Guid>> CreateTicketAsync(CreateTicketRequest request)
        {
            var errors = CreateTicketRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<Guid>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            string userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Result<Guid>.Fail("Invalid User ID.", "Invalid User ID", ResultType.Unauthorized);
            }
            var ticket = new SupportTicket
            {
                UserId = userId,
                Subject = request.Subject,
                Description = request.Description,
                Status = TicketStatus.Open,
                Priority = request.Priority,
                MobileNo = request.MobileNo,
                DepartmentId = request.DepartmentId,
                CategoryTypeId = request.CategoryTypeId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<SupportTicket>().AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(ticket.Id, "Ticket created successfully", ResultType.Success);
        }

        public async Task<Result<SupportTicketResponse>> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _unitOfWork.Repository<SupportTicket>().GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return Result<SupportTicketResponse>.Fail("Ticket not found", "Ticket not found", ResultType.NotFound);
            }

            var response = new SupportTicketResponse
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

            return Result<SupportTicketResponse>.Ok(response, "Ticket retrieved successfully", ResultType.Success);
        }

        public async Task<Result<string>> UpdateTicketStatusAsync(Guid ticketId, UpdateTicketRequest request)
        {
            var ticket = await _unitOfWork.Repository<SupportTicket>().GetByIdAsync(ticketId);

            if (ticket == null)
            {
                return Result<string>.Fail("Ticket not found", "Ticket not found", ResultType.NotFound);
            }

            ticket.Status = request.Status;

            if (request.Status == TicketStatus.Resolved)
            {
                ticket.ResolvedAt = DateTime.UtcNow;  // Set the resolved time when status is updated to Resolved
            }

            _unitOfWork.Repository<SupportTicket>().Update(ticket);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok("Ticket status updated successfully", "Ticket status updated", ResultType.Success);
        }

        public async Task<Result<IEnumerable<SupportTicketResponse>>> GetTicketsByUserIdAsync(Guid userId)
        {
            var tickets = await _unitOfWork.Repository<SupportTicket>().Where(t => t.UserId == userId);

            if (!tickets.Any())
            {
                return Result<IEnumerable<SupportTicketResponse>>.Fail("No tickets found for this user.", "No tickets found", ResultType.NotFound);
            }

            var response = tickets.Select(ticket => new SupportTicketResponse
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                Priority = ticket.Priority,
                MobileNo = ticket.MobileNo,
                DepartmentId = ticket.DepartmentId,
                CategoryTypeId = ticket.CategoryTypeId,
                ResolvedAt = ticket.ResolvedAt
            }).ToList();

            return Result<IEnumerable<SupportTicketResponse>>.Ok(response, "Tickets retrieved successfully", ResultType.Success);
        }

        public async Task<Result<IEnumerable<SupportTicketResponse>>> GetAllTicketsAsync()
        {
            var tickets = await _unitOfWork.Repository<SupportTicket>().GetAllAsync();

            if (!tickets.Any())
            {
                return Result<IEnumerable<SupportTicketResponse>>.Fail("No tickets found.", "No tickets found", ResultType.NotFound);
            }

            var response = tickets.Select(ticket => new SupportTicketResponse
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
            }).ToList();

            return Result<IEnumerable<SupportTicketResponse>>.Ok(response, "Tickets retrieved successfully", ResultType.Success);
        }
    }
}
