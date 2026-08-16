using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments.FindAsync(new object?[] { request.Id }, cancellationToken);

            if (department == null)
            {
                throw new NotFoundException("Department not found");
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Department deleted successfully");
        }
    }
}
