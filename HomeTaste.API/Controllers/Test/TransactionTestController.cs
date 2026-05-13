using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Test.MealAndMealCategory;
using HomeTaste.Application.DTOs.Test.UnitAndMealCategory;
using HomeTaste.Application.Interfaces.Test;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTestController : ControllerBase
    {
        private readonly ITransactionTestService _testService;

        public TransactionTestController(ITransactionTestService testService)
        {
            _testService = testService;
        }

        // Create a new unit and meal category
        [HttpPost("create-unit-meal-category")]
        public async Task<IActionResult> CreateUnitAndMealCategory([FromBody] UnitAndMealCategoryRequest request)
        {
            var result = await _testService.CreateUnitAndMealCategoryAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }


        // Create Meal and MealCategory
        [HttpPost("create-mealCategory-meal")]
        public async Task<IActionResult> CreateMealAndCategory([FromBody] MealAndMealCategoryRequest request)
        {
            // Call the service to create both Meal and MealCategory
            var result = await _testService.CreateMealAndMealCategoryAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
