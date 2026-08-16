using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.HardDeleteUnit
{
    public class HardDeleteUnitCommandHandler : IRequestHandler<HardDeleteUnitCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HardDeleteUnitCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(HardDeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var unitResponse = await _unitOfWork.Repository<UnitEntity>().GetByIdAsync(request.Id,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });
            if (unitResponse == null)
            {
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            var unit = new UnitEntity
            {
                Id = unitResponse.Id
            };

            _unitOfWork.Repository<UnitEntity>().Remove(unit);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Unit deleted successfully", ResultType.Success);
        }
    }
}
