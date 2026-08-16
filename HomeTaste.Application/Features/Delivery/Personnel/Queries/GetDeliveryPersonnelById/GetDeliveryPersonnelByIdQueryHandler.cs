using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetDeliveryPersonnelById
{
    public class GetDeliveryPersonnelByIdQueryHandler : IRequestHandler<GetDeliveryPersonnelByIdQuery, Result<DeliveryPersonnelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetDeliveryPersonnelByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryPersonnelResponse>> Handle(GetDeliveryPersonnelByIdQuery request, CancellationToken cancellationToken)
        {
            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            return Result<DeliveryPersonnelResponse>.Ok(DeliveryMapper.ToResponse(personnel), "Delivery personnel retrieved successfully.");
        }
    }
}
