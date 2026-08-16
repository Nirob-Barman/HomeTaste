using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommand : IRequest<Result<TaskResponse>>
    {
        public Guid Id { get; set; }
        public TaskRequest Request { get; set; }

        public UpdateTaskCommand(Guid id, TaskRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
