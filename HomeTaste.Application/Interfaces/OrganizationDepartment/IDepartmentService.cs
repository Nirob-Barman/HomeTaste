using HomeTaste.Application.DTOs.OrganizationDepartment;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.OrganizationDepartment
{
    public interface IDepartmentService
    {
        Task<Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>> GetAllDepartmentsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<DepartmentResponse>> GetDepartmentByIdAsync(Guid id);
        Task<Result<DepartmentResponse>> CreateDepartmentAsync(DepartmentRequest departmentRequest);
        Task<Result<DepartmentResponse>> UpdateDepartmentAsync(Guid id, DepartmentRequest departmentRequest);
        Task<Result<bool>> DeleteDepartmentAsync(Guid id);
    }
}
