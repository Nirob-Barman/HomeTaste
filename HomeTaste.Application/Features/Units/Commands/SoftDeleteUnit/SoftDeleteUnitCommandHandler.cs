using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.SoftDeleteUnit
{
    public class SoftDeleteUnitCommandHandler : IRequestHandler<SoftDeleteUnitCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SoftDeleteUnitCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(SoftDeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _unitOfWork.Repository<UnitEntity>().GetByIdAsync(request.Id);

            if (unit == null || unit.DeletedAt != null)
            {
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            unit.DeletedAt = DateTime.UtcNow;
            //unit.DeletedBy = Guid.NewGuid(); // Replace with logged-in user id

            _unitOfWork.Repository<UnitEntity>().Update(unit);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Unit soft deleted successfully", ResultType.Success);
        }
    }
}
