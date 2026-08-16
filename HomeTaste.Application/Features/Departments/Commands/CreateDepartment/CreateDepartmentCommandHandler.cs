using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.OrganizationDepartment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DepartmentResponse>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _context.Departments
                .Where(d => d.Name == request.Name)
                .Select(d => new DepartmentResponse { Id = d.Id, Name = d.Name, Description = d.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDepartment != null)
            {
                throw new ConflictException("Department already exists.");
            }

            var department = Department.Create(request.Name, request.Description);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            var departmentResponse = new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return Result<DepartmentResponse>.Ok(departmentResponse, "Department created successfully");
        }
    }
}
