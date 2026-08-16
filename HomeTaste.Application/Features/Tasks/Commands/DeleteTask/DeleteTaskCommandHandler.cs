using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (task == null)
                throw new NotFoundException("Task not found");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Task deleted successfully");
        }
    }
}
