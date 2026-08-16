using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.SoftDeleteUnit
{
    public class SoftDeleteUnitCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public SoftDeleteUnitCommand(Guid id)
        {
            Id = id;
        }
    }
}
