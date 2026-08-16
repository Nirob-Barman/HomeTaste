using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.BulkInsertTasks
{
    public record BulkInsertTasksCommand : IRequest<Result<int>>;
}
