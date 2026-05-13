using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Support
{
    public interface ICategoryTypeService
    {
        Task<Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>> GetAllCategoryTypesAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<CategoryTypeResponse>> GetCategoryTypeByIdAsync(Guid id);
        Task<Result<CategoryTypeResponse>> CreateCategoryTypeAsync(CategoryTypeRequest categoryTypeRequest);
        Task<Result<CategoryTypeResponse>> UpdateCategoryTypeAsync(Guid id, CategoryTypeRequest categoryTypeRequest);
        Task<Result<bool>> DeleteCategoryTypeAsync(Guid id);
    }
}
