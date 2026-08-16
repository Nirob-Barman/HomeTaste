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
                return Result<bool>.Fail("Department not found", "Department not found", ResultType.NotFound);
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Department deleted successfully", ResultType.Success);
        }
    }
}
