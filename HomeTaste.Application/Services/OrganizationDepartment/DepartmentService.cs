using HomeTaste.Application.DTOs.OrganizationDepartment;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.OrganizationDepartment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.OrganizationDepartment;

namespace HomeTaste.Application.Services.OrganizationDepartment
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<DepartmentResponse>>> GetAllDepartmentsAsync()
        {
            var departmentResponses = await _unitOfWork.Repository<Department>().GetAllAsync(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            });

            if (departmentResponses == null || !departmentResponses.Any())
            {
                return Result<IEnumerable<DepartmentResponse>>.Fail("No departments found", "No departments found", ResultType.NotFound);
            }

            return Result<IEnumerable<DepartmentResponse>>.Ok(departmentResponses, "Departments retrieved successfully", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>> GetAllDepartmentsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!)
        {
            var departmentResponses = await _unitOfWork.Repository<Department>().GetAllAsync(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            });

            //var totalCount = departmentResponses.Count();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                departmentResponses = departmentResponses.Where(department =>
                    department.Name!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    department.Description!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = departmentResponses.Count();

            var pagedDepartments = departmentResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            var currentPageCount = pagedDepartments.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<DepartmentResponse>>
            {
                Data = pagedDepartments,
                MetaData = paginationMeta
            };

            if (!pagedDepartments.Any())
            {
                return Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>.Fail("No departments found", "No departments found", ResultType.NotFound);
            }

            return Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>.Ok(response, "Departments retrieved successfully", ResultType.Success);
        }

        // Get department by Id
        public async Task<Result<DepartmentResponse>> GetDepartmentByIdAsync(Guid id)
        {
            var departmentResponse = await _unitOfWork.Repository<Department>().GetByIdAsync(id, d => new DepartmentResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            });

            if (departmentResponse == null)
            {
                return Result<DepartmentResponse>.Fail("Department not found", "Department not found", ResultType.NotFound);
            }

            return Result<DepartmentResponse>.Ok(departmentResponse, "Department retrieved successfully", ResultType.Success);
        }

        // Create a new department
        public async Task<Result<DepartmentResponse>> CreateDepartmentAsync(DepartmentRequest departmentRequest)
        {
            var existingDepartment = await _unitOfWork.Repository<Department>().FirstOrDefaultAsync(d => d.Name == departmentRequest.Name,
                d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                });

            if (existingDepartment != null)
            {
                return Result<DepartmentResponse>.Fail("Department already exists.", "Duplicate department", ResultType.Conflict);
            }

            var department = new Department
            {
                Name = departmentRequest.Name,
                Description = departmentRequest.Description
            };

            await _unitOfWork.Repository<Department>().AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            var departmentResponse = new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return Result<DepartmentResponse>.Ok(departmentResponse, "Department created successfully", ResultType.Success);
        }

        // Update an existing department
        public async Task<Result<DepartmentResponse>> UpdateDepartmentAsync(Guid id, DepartmentRequest departmentRequest)
        {
            var departmentResponse = await _unitOfWork.Repository<Department>().GetByIdAsync(id,
                d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                });

            if (departmentResponse == null)
            {
                return Result<DepartmentResponse>.Fail("Department not found", "Department not found", ResultType.NotFound);
            }

            var existingDepartment = await _unitOfWork.Repository<Department>().FirstOrDefaultAsync(d =>
                d.Name == departmentRequest.Name && d.Id != id,
                d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                });

            if (existingDepartment != null)
            {
                return Result<DepartmentResponse>.Fail("Department with the same name already exists.", "Duplicate department", ResultType.Conflict);
            }

            var department = new Department
            {
                Id = departmentResponse.Id,
                Name = departmentRequest.Name ?? departmentResponse.Name,
                Description = departmentRequest.Description ?? departmentResponse.Description
            };

            _unitOfWork.Repository<Department>().Update(department);
            await _unitOfWork.SaveChangesAsync();

            var updatedDepartmentResponse = new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

            return Result<DepartmentResponse>.Ok(updatedDepartmentResponse, "Department updated successfully", ResultType.Success);
        }

        // Delete a department
        public async Task<Result<bool>> DeleteDepartmentAsync(Guid id)
        {
            var departmentResponse = await _unitOfWork.Repository<Department>().GetByIdAsync(id,
                d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description
                });

            if (departmentResponse == null)
            {
                return Result<bool>.Fail("Department not found", "Department not found", ResultType.NotFound);
            }

            var department = new Department
            {
                Id = departmentResponse.Id
            };

            _unitOfWork.Repository<Department>().Remove(department);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Department deleted successfully", ResultType.Success);
        }
    }
}
