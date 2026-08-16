using HomeTaste.Application.Features.Meals;
using HomeTaste.Application.Features.Meals.Commands.BulkInsertMeals;
using HomeTaste.Application.Features.Meals.Commands.CreateMeal;
using HomeTaste.Application.Features.Meals.Commands.DeleteMeal;
using HomeTaste.Application.Features.Meals.Commands.UpdateMeal;
using HomeTaste.Application.Features.Meals.Queries.GetAllMeals;
using HomeTaste.Application.Features.Meals.Queries.GetMealById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get all meals
        [HttpGet]
        public async Task<IActionResult> GetAllMeals([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!, [FromQuery] Guid? categoryId = null)
        {
            var result = await _mediator.Send(new GetAllMealsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm, CategoryId = categoryId });
            return Ok(result);
        }

        // Get meal by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealById(Guid id)
        {
            var result = await _mediator.Send(new GetMealByIdQuery(id));
            return Ok(result);
        }

        // Create a new meal
        [HttpPost]
        public async Task<IActionResult> CreateMeal([FromForm] MealRequest mealRequest)
        {
            var result = await _mediator.Send(new CreateMealCommand(
                mealRequest.Name,
                mealRequest.Description,
                mealRequest.Price,
                mealRequest.CategoryId,
                mealRequest.ImageUrl,
                mealRequest.IsAvailable,
                mealRequest.PreparationTime,
                mealRequest.DiscountPrice,
                mealRequest.Calories));
            return Ok(result);
        }

        // Update an existing meal
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(Guid id, [FromBody] MealRequest mealRequest)
        {
            var result = await _mediator.Send(new UpdateMealCommand(
                id,
                mealRequest.Name,
                mealRequest.Description,
                mealRequest.Price,
                mealRequest.CategoryId,
                mealRequest.ImageUrl,
                mealRequest.IsAvailable,
                mealRequest.PreparationTime,
                mealRequest.DiscountPrice,
                mealRequest.Calories));
            return Ok(result);
        }

        // Delete a meal
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(Guid id)
        {
            var result = await _mediator.Send(new DeleteMealCommand(id));
            return Ok(result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertMeals()
        {
            var result = await _mediator.Send(new BulkInsertMealsCommand());
            return Ok(result);
        }
    }
}
