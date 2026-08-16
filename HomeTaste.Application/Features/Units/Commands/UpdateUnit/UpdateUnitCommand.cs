using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<Result<UnitResponse>>
    {
        public Guid Id { get; set; }
        public UnitRequest UnitRequest { get; set; }

        public UpdateUnitCommand(Guid id, UnitRequest unitRequest)
        {
            Id = id;
            UnitRequest = unitRequest;
        }
    }
}
