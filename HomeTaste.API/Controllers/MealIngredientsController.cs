using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealIngredientsController : ControllerBase
    {
        private readonly IMealIngredientService _mealIngredientService;

        public MealIngredientsController(IMealIngredientService mealIngredientService)
        {
            _mealIngredientService = mealIngredientService;
        }

        // Get all meal ingredients
        [HttpGet]
        public async Task<IActionResult> GetAllMealIngredients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mealIngredientService.GetAllMealIngredientsAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get meal ingredient by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealIngredientById(Guid id)
        {
            var result = await _mealIngredientService.GetMealIngredientByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new meal ingredient
        [HttpPost]
        public async Task<IActionResult> CreateMealIngredient([FromBody] MealIngredientRequest mealIngredientRequest)
        {
            var result = await _mealIngredientService.CreateMealIngredientAsync(mealIngredientRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing meal ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealIngredient(Guid id, [FromBody] MealIngredientRequest mealIngredientRequest)
        {
            var result = await _mealIngredientService.UpdateMealIngredientAsync(id, mealIngredientRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a meal ingredient
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealIngredient(Guid id)
        {
            var result = await _mealIngredientService.DeleteMealIngredientAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }


        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertMealIngredients()
        {
            var result = await _mealIngredientService.BulkInsertPredefinedMealIngredientsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
