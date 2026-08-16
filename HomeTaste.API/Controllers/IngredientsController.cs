using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.Features.Ingredients;
using HomeTaste.Application.Features.Ingredients.Commands.BulkInsertIngredients;
using HomeTaste.Application.Features.Ingredients.Commands.CreateIngredient;
using HomeTaste.Application.Features.Ingredients.Commands.DeleteIngredient;
using HomeTaste.Application.Features.Ingredients.Commands.UpdateIngredient;
using HomeTaste.Application.Features.Ingredients.Queries.GetAllIngredients;
using HomeTaste.Application.Features.Ingredients.Queries.GetIngredientById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IngredientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all ingredients
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="sortBy">Valid values: "Id", "Name", "CreatedAt"</param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllIngredients([FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string searchTerm = null!,
            [FromQuery] string sortBy = "Id",
            [FromQuery] string sortOrder = "ASC")
        {
            var result = await _mediator.Send(new GetAllIngredientsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm, SortBy = sortBy, SortOrder = sortOrder });
            return Ok(result);
        }

        // Get ingredient by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredientById(Guid id)
        {
            var result = await _mediator.Send(new GetIngredientByIdQuery(id));
            return Ok(result);
        }

        // Create a new ingredient
        [HttpPost]
        public async Task<IActionResult> CreateIngredient([FromForm] IngredientRequest ingredientRequest, IFormFile? image)
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
            var result = await _mediator.Send(new CreateIngredientCommand(ingredientRequest, fileDto));
            return Ok(result);
        }

        // Update an existing ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] IngredientRequest ingredientRequest)
        {
            var result = await _mediator.Send(new UpdateIngredientCommand(id, ingredientRequest));
            return Ok(result);
        }

        // Delete an ingredient
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(Guid id)
        {
            var result = await _mediator.Send(new DeleteIngredientCommand(id));
            return Ok(result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedIngredients()
        {
            var result = await _mediator.Send(new BulkInsertIngredientsCommand());
            return Ok(result);
        }
    }
}
