using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQuery : IRequest<Result<TaskResponse>>
    {
        public Guid Id { get; set; }

        public GetTaskByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
