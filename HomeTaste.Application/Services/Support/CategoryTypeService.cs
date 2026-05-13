using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.Support;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Support;

namespace HomeTaste.Application.Services.Support
{
    public class CategoryTypeService : ICategoryTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all category types with pagination and search
        public async Task<Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>> GetAllCategoryTypesAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!)
        {
            var categoryTypes = await _unitOfWork.Repository<CategoryType>().GetAllAsync(ct => new CategoryTypeResponse
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description
            });

            //var totalCount = categoryTypes.Count();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                categoryTypes = categoryTypes.Where(ct =>
                    ct.Name!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    ct.Description!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = categoryTypes.Count();

            var pagedCategoryTypes = categoryTypes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            var currentPageCount = pagedCategoryTypes.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<CategoryTypeResponse>>
            {
                Data = pagedCategoryTypes,
                MetaData = paginationMeta
            };

            if (!pagedCategoryTypes.Any())
            {
                return Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>.Fail("No category types found", "No category types found", ResultType.NotFound);
            }

            return Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>.Ok(response, "Category types retrieved successfully", ResultType.Success);
        }

        // Get category type by Id
        public async Task<Result<CategoryTypeResponse>> GetCategoryTypeByIdAsync(Guid id)
        {
            var categoryType = await _unitOfWork.Repository<CategoryType>().GetByIdAsync(id, ct => new CategoryTypeResponse
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description
            });

            if (categoryType == null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            return Result<CategoryTypeResponse>.Ok(categoryType, "Category type retrieved successfully", ResultType.Success);
        }

        // Create a new category type
        public async Task<Result<CategoryTypeResponse>> CreateCategoryTypeAsync(CategoryTypeRequest categoryTypeRequest)
        {
            var existingCategoryType = await _unitOfWork.Repository<CategoryType>().FirstOrDefaultAsync(ct => ct.Name == categoryTypeRequest.Name,
                ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                });

            if (existingCategoryType != null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type with the same name already exists.", "Duplicate category type", ResultType.Conflict);
            }

            var categoryType = new CategoryType
            {
                Name = categoryTypeRequest.Name,
                Description = categoryTypeRequest.Description
            };

            await _unitOfWork.Repository<CategoryType>().AddAsync(categoryType);
            await _unitOfWork.SaveChangesAsync();

            var categoryTypeResponse = new CategoryTypeResponse
            {
                Id = categoryType.Id,
                Name = categoryType.Name,
                Description = categoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(categoryTypeResponse, "Category type created successfully", ResultType.Success);
        }

        // Update an existing category type
        public async Task<Result<CategoryTypeResponse>> UpdateCategoryTypeAsync(Guid id, CategoryTypeRequest categoryTypeRequest)
        {
            var categoryType = await _unitOfWork.Repository<CategoryType>().GetByIdAsync(id, ct => new CategoryTypeResponse
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description
            });

            if (categoryType == null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            var existingCategoryType = await _unitOfWork.Repository<CategoryType>().FirstOrDefaultAsync(ct =>
                ct.Name == categoryTypeRequest.Name && ct.Id != id,
                ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                });

            if (existingCategoryType != null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type with the same name already exists.", "Duplicate category type", ResultType.Conflict);
            }

            var updatedCategoryType = new CategoryType
            {
                Id = categoryType.Id,
                Name = categoryTypeRequest.Name ?? categoryType.Name,
                Description = categoryTypeRequest.Description ?? categoryType.Description
            };

            _unitOfWork.Repository<CategoryType>().Update(updatedCategoryType);
            await _unitOfWork.SaveChangesAsync();

            var updatedCategoryTypeResponse = new CategoryTypeResponse
            {
                Id = updatedCategoryType.Id,
                Name = updatedCategoryType.Name,
                Description = updatedCategoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(updatedCategoryTypeResponse, "Category type updated successfully", ResultType.Success);
        }

        // Delete a category type
        public async Task<Result<bool>> DeleteCategoryTypeAsync(Guid id)
        {
            var categoryType = await _unitOfWork.Repository<CategoryType>().GetByIdAsync(id,
                ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                });

            if (categoryType == null)
            {
                return Result<bool>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            var categoryTypeToDelete = new CategoryType
            {
                Id = categoryType.Id
            };

            _unitOfWork.Repository<CategoryType>().Remove(categoryTypeToDelete);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Category type deleted successfully", ResultType.Success);
        }
    }
}
