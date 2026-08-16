using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetUnitById
{
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnitByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UnitResponse>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            var unitResponse = await _unitOfWork.Repository<UnitEntity>().GetByIdAsync(request.Id, u => new UnitResponse
            {
                Id = u.Id,
                Name = u.Name,
                Abbreviation = u.Abbreviation
            });

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            return Result<UnitResponse>.Ok(unitResponse, "Unit retrieved successfully", ResultType.Success);
        }
    }
}
