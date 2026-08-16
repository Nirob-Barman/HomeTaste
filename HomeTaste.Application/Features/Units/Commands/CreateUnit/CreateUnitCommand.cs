using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<Result<UnitResponse>>
    {
        public UnitRequest UnitRequest { get; set; }

        public CreateUnitCommand(UnitRequest unitRequest)
        {
            UnitRequest = unitRequest;
        }
    }
}
