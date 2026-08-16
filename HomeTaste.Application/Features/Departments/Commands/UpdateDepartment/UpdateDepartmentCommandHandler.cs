using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DepartmentResponse>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var departmentRequest = request.DepartmentRequest;

            var department = await _context.Departments.FindAsync(new object?[] { id }, cancellationToken);

            if (department == null)
            {
                throw new NotFoundException("Department not found");
            }

            var existingDepartment = await _context.Departments
                .Where(d => d.Name == departmentRequest.Name && d.Id != id)
                .Select(d => new DepartmentResponse { Id = d.Id, Name = d.Name, Description = d.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDepartment != null)
            {
                throw new ConflictException("Department with the same name already exists.");
            }

            department.UpdateDetails(departmentRequest.Name, departmentRequest.Description);

            await _context.SaveChangesAsync(cancellationToken);

            var updatedDepartmentResponse = new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return Result<DepartmentResponse>.Ok(updatedDepartmentResponse, "Department updated successfully");
        }
    }
}
