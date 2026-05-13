using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealsController(IMealService mealService)
        {
            _mealService = mealService;
        }

        // Get all meals
        [HttpGet]
        public async Task<IActionResult> GetAllMeals([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mealService.GetAllMealsAsync(pageNumber,pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get meal by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealById(Guid id)
        {
            var result = await _mealService.GetMealByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new meal
        [HttpPost]
        public async Task<IActionResult> CreateMeal([FromForm] MealRequest mealRequest, IFormFile? image)
        {
            FileUploadDto? fileDto = null;
            if (image != null && image.Length > 0)
            {
                fileDto = new FileUploadDto
                {
                    Content = image.OpenReadStream(),
                    FileName = image.FileName,
                    ContentType = image.ContentType,
                    Size = image.Length
                };
            }
            var result = await _mealService.CreateMealAsync(mealRequest, fileDto);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing meal
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(Guid id, [FromBody] MealRequest mealRequest)
        {
            var result = await _mealService.UpdateMealAsync(id, mealRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a meal
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(Guid id)
        {
            var result = await _mealService.DeleteMealAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertMeals()
        {
            var result = await _mealService.BulkInsertPredefinedMealsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
