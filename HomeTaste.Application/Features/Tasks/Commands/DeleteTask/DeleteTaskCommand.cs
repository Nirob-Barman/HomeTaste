using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(Guid Id) : IRequest<Result<bool>>;
}
