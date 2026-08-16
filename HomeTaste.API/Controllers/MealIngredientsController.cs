using HomeTaste.Application.Features.MealIngredients;
using HomeTaste.Application.Features.MealIngredients.Commands.BulkInsertMealIngredients;
using HomeTaste.Application.Features.MealIngredients.Commands.CreateMealIngredient;
using HomeTaste.Application.Features.MealIngredients.Commands.DeleteMealIngredient;
using HomeTaste.Application.Features.MealIngredients.Commands.UpdateMealIngredient;
using HomeTaste.Application.Features.MealIngredients.Queries.GetAllMealIngredients;
using HomeTaste.Application.Features.MealIngredients.Queries.GetMealIngredientById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealIngredientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealIngredientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get all meal ingredients
        [HttpGet]
        public async Task<IActionResult> GetAllMealIngredients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllMealIngredientsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });
            return Ok(result);
        }

        // Get meal ingredient by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealIngredientById(Guid id)
        {
            var result = await _mediator.Send(new GetMealIngredientByIdQuery(id));
            return Ok(result);
        }

        // Create a new meal ingredient
        [HttpPost]
        public async Task<IActionResult> CreateMealIngredient([FromBody] MealIngredientRequest mealIngredientRequest)
        {
            var result = await _mediator.Send(new CreateMealIngredientCommand(mealIngredientRequest.MealId, mealIngredientRequest.IngredientId, mealIngredientRequest.Quantity, mealIngredientRequest.UnitId));
            return Ok(result);
        }

        // Update an existing meal ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealIngredient(Guid id, [FromBody] MealIngredientRequest mealIngredientRequest)
        {
            var result = await _mediator.Send(new UpdateMealIngredientCommand(id, mealIngredientRequest.MealId, mealIngredientRequest.IngredientId, mealIngredientRequest.Quantity, mealIngredientRequest.UnitId));
            return Ok(result);
        }

        // Delete a meal ingredient
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealIngredient(Guid id)
        {
            var result = await _mediator.Send(new DeleteMealIngredientCommand(id));
            return Ok(result);
        }


        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertMealIngredients()
        {
            var result = await _mediator.Send(new BulkInsertMealIngredientsCommand());
            return Ok(result);
        }
    }
}
