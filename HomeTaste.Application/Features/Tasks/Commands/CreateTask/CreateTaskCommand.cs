using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommand : IRequest<Result<TaskResponse>>
    {
        public TaskRequest Request { get; set; }

        public CreateTaskCommand(TaskRequest request)
        {
            Request = request;
        }
    }
}
