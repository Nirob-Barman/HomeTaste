using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Units.Queries.GetUnitById
{
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetUnitByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UnitResponse>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            var unitResponse = await _context.Units
                .Where(u => u.Id == request.Id)
                .Select(u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (unitResponse == null)
            {
                throw new NotFoundException("Unit not found");
            }

            return Result<UnitResponse>.Ok(unitResponse, "Unit retrieved successfully");
        }
    }
}
