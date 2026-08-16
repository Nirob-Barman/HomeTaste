using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.HardDeleteUnit
{
    public class HardDeleteUnitCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public HardDeleteUnitCommand(Guid id)
        {
            Id = id;
        }
    }
}
