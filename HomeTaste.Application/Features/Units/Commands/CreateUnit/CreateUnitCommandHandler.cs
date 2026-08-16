using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<UnitResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUnitCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UnitResponse>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var unitRequest = request.UnitRequest;

            var existingUnit = await _unitOfWork.Repository<UnitEntity>().FirstOrDefaultAsync(u => u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });

            if (existingUnit != null)
            {
                return Result<UnitResponse>.Fail("Unit already exists with the same name or abbreviation.", "Duplicate unit", ResultType.Conflict);
            }

            var unit = new UnitEntity
            {
                Name = unitRequest.Name,
                Abbreviation = unitRequest.Abbreviation
            };

            await _unitOfWork.Repository<UnitEntity>().AddAsync(unit);
            await _unitOfWork.SaveChangesAsync();

            var unitResponse = new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            };

            return Result<UnitResponse>.Ok(unitResponse, "Unit created successfully", ResultType.Success);
        }
    }
}
