using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.Features.MealCategories.Commands.BulkInsertMealCategories;
using HomeTaste.Application.Features.MealCategories.Commands.CreateMealCategory;
using HomeTaste.Application.Features.MealCategories.Commands.DeleteMealCategory;
using HomeTaste.Application.Features.MealCategories.Commands.UpdateMealCategory;
using HomeTaste.Application.Features.MealCategories.Queries.GetAllMealCategories;
using HomeTaste.Application.Features.MealCategories.Queries.GetMealCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
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
            var result = await _mediator.Send(new GetAllMealCategoriesQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm, SortBy = sortBy, SortOrder = sortOrder });
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get meal category by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealCategoryById(Guid id)
        {
            var result = await _mediator.Send(new GetMealCategoryByIdQuery(id));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new meal category
        [HttpPost]
        public async Task<IActionResult> CreateMealCategory([FromBody] MealCategoryRequest mealCategoryRequest)
        {
            var result = await _mediator.Send(new CreateMealCategoryCommand(mealCategoryRequest));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing meal category
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealCategory(Guid id, [FromBody] MealCategoryRequest mealCategoryRequest)
        {
            var result = await _mediator.Send(new UpdateMealCategoryCommand(id, mealCategoryRequest));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a meal category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealCategory(Guid id)
        {
            var result = await _mediator.Send(new DeleteMealCategoryCommand(id));
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedMealCategories()
        {
            var result = await _mediator.Send(new BulkInsertMealCategoriesCommand());
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
