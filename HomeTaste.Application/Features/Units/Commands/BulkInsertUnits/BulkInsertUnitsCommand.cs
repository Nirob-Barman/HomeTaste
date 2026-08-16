using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.BulkInsertUnits
{
    public class BulkInsertUnitsCommand : IRequest<Result<int>>
    {
    }
}
