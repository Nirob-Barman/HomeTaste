using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetDepartmentByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DepartmentResponse>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var departmentResponse = await _context.Departments
                .Where(d => d.Id == request.Id)
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (departmentResponse == null)
            {
                throw new NotFoundException("Department not found");
            }

            return Result<DepartmentResponse>.Ok(departmentResponse, "Department retrieved successfully");
        }
    }
}
