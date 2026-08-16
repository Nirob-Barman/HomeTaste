using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.BulkInsertTasks
{
    public class BulkInsertTasksCommand : IRequest<Result<int>>
    {
    }
}
