using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.BulkInsertUnits
{
    public record BulkInsertUnitsCommand : IRequest<Result<int>>;
}
