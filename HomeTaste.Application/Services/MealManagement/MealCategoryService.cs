using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class MealCategoryService : IMealCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MealCategory> _mealCategoryRepository;

        public MealCategoryService(IUnitOfWork unitOfWork, IRepository<MealCategory> mealCategoryRepository)
        {
            _unitOfWork = unitOfWork;
            _mealCategoryRepository = mealCategoryRepository;
        }

        public async Task<Result<IEnumerable<MealCategoryResponse>>> GetAllMealCategoriesAsync(int pageNumber, int pageSize)
        {
            var query = _mealCategoryRepository.GetAllAsQueryable();
            //var totalCount = await _mealCategoryRepository.CountAsync(query);
            var paginatedQuery = _mealCategoryRepository.PaginateAsQueryable(query, pageNumber, pageSize);
            //var currentPageCount = await _mealCategoryRepository.CountAsync(paginatedQuery);
            //var mealCategories = await _mealCategoryRepository.ToListAsync(paginatedQuery);
            var mealCategories = await _mealCategoryRepository.ToEnumerableAsync(paginatedQuery,
                mealCategory => new MealCategoryResponse
                {
                    Id = mealCategory.Id,
                    Name = mealCategory.Name,
                    Description = mealCategory.Description,
                    ImageUrl = mealCategory.ImageUrl
                });
            //var currentPageCount = mealCategories.Count();

            //var paginatedQuery = _mealCategoryRepository.PaginateAsQueryable(pageNumber, pageSize);
            //var mealCategories = await _mealCategoryRepository.ToEnumerableAsync(paginatedQuery,
            //    mealCategory => new MealCategoryResponse
            //    {
            //        Id = mealCategory.Id,
            //        Name = mealCategory.Name,
            //        Description = mealCategory.Description
            //    });



            //var mealCategories = await _mealCategoryRepository.GetPagedAsync(pageNumber,
            //    pageSize,
            //    mealCategory => new MealCategoryResponse
            //    {
            //        Id = mealCategory.Id,
            //        Name = mealCategory.Name,
            //        Description = mealCategory.Description
            //    });

            return Result<IEnumerable<MealCategoryResponse>>.Ok(mealCategories, "Meal categories retrieved successfully", ResultType.Success);
        }


        public async Task<Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>> GetAllMealCategoriesAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!, string sortBy = "Id", string sortOrder = "ASC")
        {
            var query = _mealCategoryRepository.GetAllAsQueryable();
            //var totalCount = await _mealCategoryRepository.CountAsync(query);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(mealCategory =>
                    mealCategory.Name!.Contains(searchTerm) ||
                    mealCategory.Description!.Contains(searchTerm)
                );
            }

            var validSortColumns = new List<string> { "Id", "Name" , "CreatedAt" };
            if (!validSortColumns.Contains(sortBy))
            {
                return Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>.Fail("Invalid sort column", "Invalid sort column.", ResultType.Failure);
            }
            var totalCount = await _mealCategoryRepository.CountAsync(query);
            query = _mealCategoryRepository.OrderBy(query, sortBy, sortOrder, validSortColumns);

            // Apply pagination to get the current page's query
            var paginatedQuery = _mealCategoryRepository.PaginateAsQueryable(query, pageNumber, pageSize);

            // Get the actual paged meal categories
            var mealCategories = await _mealCategoryRepository.ToListAsync(paginatedQuery,
                mealCategory => new MealCategoryResponse
                {
                    Id = mealCategory.Id,
                    Name = mealCategory.Name,
                    Description = mealCategory.Description,
                    ImageUrl = mealCategory.ImageUrl
                });

            //var hasPreviousPage = pageNumber > 1;
            //var hasNextPage = pageNumber < totalPages;

            //var isFirstPage = pageNumber == 1;
            //var isLastPage = pageNumber == totalPages;

            // Construct the pagination metadata and return along with data
            //var response = new PaginatedResponse<IEnumerable<MealCategoryResponse>>
            //{
            //    Data = mealCategories,
            //    MetaData = new PaginationMeta
            //    {
            //        TotalCount = totalCount,
            //        PageNumber = pageNumber,
            //        PageSize = pageSize,
            //        TotalPages = totalPages,
            //        CurrentPageCount = mealCategories.Count(),
            //        HasPreviousPage = hasPreviousPage,
            //        HasNextPage = hasNextPage,
            //        IsFirstPage = isFirstPage,
            //        IsLastPage = isLastPage
            //    }
            //};


            // Calculate the current page count
            var currentPageCount = mealCategories.Count();
            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            // Update pagination metadata to include current page count
            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<MealCategoryResponse>>
            {
                Data = mealCategories,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>.Ok(response, "Meal categories retrieved successfully", ResultType.Success);
        }


        public async Task<Result<MealCategoryResponse>> GetMealCategoryByIdAsync(Guid id)
        {
            var mealCategory = await _mealCategoryRepository.GetByIdAsync(id);
            if (mealCategory == null)
                return Result<MealCategoryResponse>.Fail("Meal category not found", "Meal category not found", ResultType.NotFound);

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category retrieved successfully", ResultType.Success);
        }

        public async Task<Result<MealCategoryResponse>> CreateMealCategoryAsync(MealCategoryRequest mealCategoryRequest)
        {
            var existingCategory = await _mealCategoryRepository.FirstOrDefaultAsync(c => c.Name == mealCategoryRequest.Name);

            if (existingCategory != null)
            {
                return Result<MealCategoryResponse>.Fail("Meal category with the same name already exists.", "Duplicate meal category", ResultType.Conflict);
            }

            var mealCategory = new MealCategory
            {
                Name = mealCategoryRequest.Name,
                Description = mealCategoryRequest.Description
            };

            await _mealCategoryRepository.AddAsync(mealCategory);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category created successfully", ResultType.Success);
        }

        public async Task<Result<int>> BulkInsertPredefinedMealCategoriesAsync()
        {
            try
            {
                // Predefined Bengali Meal Categories
                var mealCategories = new List<MealCategory>
                {
                    new MealCategory { Name = "Fish Curry", Description = "Meals that focus on fish as the main ingredient, often cooked in flavorful spices, mustard oil, and sometimes coconut milk.", ImageUrl = "https://example.com/images/fish_curry.jpg" },
                    new MealCategory { Name = "Vegetarian", Description = "Dishes made without meat or fish, often featuring vegetables, lentils, and spices.", ImageUrl = "https://example.com/images/vegetarian.jpg" },
                    new MealCategory { Name = "Bengali Sweets", Description = "Sweet treats that are a staple in Bengali cuisine, including yogurt-based desserts and other sugary delights.", ImageUrl = "https://example.com/images/bengali_sweets.jpg" },
                    new MealCategory { Name = "Rice and Dal", Description = "Simple, hearty meals consisting of rice and lentils, commonly paired with vegetables or chutneys.", ImageUrl = "https://example.com/images/rice_and_dal.jpg" },
                    new MealCategory { Name = "Snacks", Description = "Small, typically fried or steamed items, often served as appetizers or street food.", ImageUrl = "https://example.com/images/snacks.jpg" },
                    new MealCategory { Name = "Biryani", Description = "Fragrant rice dishes, typically made with meat (usually chicken or mutton), and rich in aromatic spices.", ImageUrl = "https://example.com/images/biryani.jpg" },
                    new MealCategory { Name = "Non-Vegetarian (Mutton/Chicken)", Description = "Meals that include meats such as chicken or mutton, often prepared in rich curries or grilled styles.", ImageUrl = "https://example.com/images/non_vegetarian.jpg" },
                    new MealCategory { Name = "Traditional Bengali Delights", Description = "Classic Bengali dishes that showcase the region's culinary heritage and flavors.", ImageUrl = "https://example.com/images/traditional_bengali_delights.jpg" },
                    new MealCategory { Name = "Comfort Food", Description = "Simple, comforting dishes that are often eaten for daily meals, including rice and lentils.", ImageUrl = "https://example.com/images/comfort_food.jpg" }
                };

                var newCategories = new List<MealCategory>();

                foreach (var category in mealCategories)
                {
                    var categoryExists = await _mealCategoryRepository.AnyAsync(c => c.Name == category.Name);

                    if (!categoryExists)
                    {
                        newCategories.Add(category);
                    }
                }

                if (!newCategories.Any())
                {
                    return Result<int>.Fail("All meal categories already exist.", "No new categories to insert", ResultType.Conflict);
                }

                // Bulk insert the new meal categories
                await _mealCategoryRepository.AddRangeAsync(newCategories);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newCategories.Count, "New meal categories successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting meal categories: {ex.Message}", "", ResultType.Failure);
            }
        }


        public async Task<Result<MealCategoryResponse>> UpdateMealCategoryAsync(Guid id, MealCategoryRequest mealCategoryRequest)
        {
            var mealCategory = await _mealCategoryRepository.GetByIdAsync(id);
            if (mealCategory == null)
                return Result<MealCategoryResponse>.Fail("Meal category not found", "Meal category not found", ResultType.NotFound);

            var existingCategory = await _mealCategoryRepository.FirstOrDefaultAsync(c => c.Name == mealCategoryRequest.Name && c.Id != id);

            if (existingCategory != null)
            {
                return Result<MealCategoryResponse>.Fail("Meal category with the same name already exists.", "Duplicate meal category", ResultType.Conflict);
            }

            mealCategory.Name = mealCategoryRequest.Name ?? mealCategory.Name;
            mealCategory.Description = mealCategoryRequest.Description ?? mealCategory.Description;

            _mealCategoryRepository.Update(mealCategory);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteMealCategoryAsync(Guid id)
        {
            var mealCategory = await _mealCategoryRepository.GetByIdAsync(id);
            if (mealCategory == null)
                return Result<bool>.Fail("Meal category not found", "Meal category not found", ResultType.NotFound);

            _mealCategoryRepository.Remove(mealCategory);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Meal category deleted successfully", ResultType.Success);
        }
    }
}
