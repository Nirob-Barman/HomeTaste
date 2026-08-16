using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetAllDeliveryPersonnel
{
    public class GetAllDeliveryPersonnelQueryHandler : IRequestHandler<GetAllDeliveryPersonnelQuery, Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDeliveryPersonnelQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>> Handle(GetAllDeliveryPersonnelQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DeliveryPersonnel.AsQueryable();
            var totalCount = await query.CountAsync(cancellationToken);

            var personnel = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new DeliveryPersonnelResponse(
                    p.Id,
                    p.UserId,
                    p.FullName,
                    p.Phone,
                    p.VehicleType,
                    p.VehicleNumber,
                    p.IsAvailable,
                    p.CurrentLatitude,
                    p.CurrentLongitude,
                    p.Rating,
                    p.TotalDeliveries,
                    p.CreatedAt))
                .ToListAsync(cancellationToken);

            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>> { Data = personnel, MetaData = meta },
                "Delivery personnel retrieved successfully.");
        }
    }
}
