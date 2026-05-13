using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealCategoriesController : ControllerBase
    {
        private readonly IMealCategoryService _mealCategoryService;

        public MealCategoriesController(IMealCategoryService mealCategoryService)
        {
            _mealCategoryService = mealCategoryService;
        }

        // Get all meal categories
        /// <summary>
        /// Get all meal categories
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="sortBy">Valid values: "Id", "Name", "CreatedDate"."</param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetAllMealCategories([FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string searchTerm = null!,            
            [FromQuery] string sortBy = "Id",
            [FromQuery] string sortOrder = "ASC")
        {
            var result = await _mealCategoryService.GetAllMealCategoriesAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get meal category by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealCategoryById(Guid id)
        {
            var result = await _mealCategoryService.GetMealCategoryByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new meal category
        [HttpPost]
        public async Task<IActionResult> CreateMealCategory([FromBody] MealCategoryRequest mealCategoryRequest)
        {
            var result = await _mealCategoryService.CreateMealCategoryAsync(mealCategoryRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing meal category
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealCategory(Guid id, [FromBody] MealCategoryRequest mealCategoryRequest)
        {
            var result = await _mealCategoryService.UpdateMealCategoryAsync(id, mealCategoryRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a meal category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealCategory(Guid id)
        {
            var result = await _mealCategoryService.DeleteMealCategoryAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedMealCategories()
        {
            var result = await _mealCategoryService.BulkInsertPredefinedMealCategoriesAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
