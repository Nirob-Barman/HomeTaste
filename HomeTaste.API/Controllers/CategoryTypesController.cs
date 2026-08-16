using HomeTaste.Application.Features.CategoryTypes;
using HomeTaste.Application.Features.CategoryTypes.Commands.CreateCategoryType;
using HomeTaste.Application.Features.CategoryTypes.Commands.DeleteCategoryType;
using HomeTaste.Application.Features.CategoryTypes.Commands.UpdateCategoryType;
using HomeTaste.Application.Features.CategoryTypes.Queries.GetAllCategoryTypes;
using HomeTaste.Application.Features.CategoryTypes.Queries.GetCategoryTypeById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get all category types with pagination and search
        [HttpGet]
        public async Task<IActionResult> GetAllCategoryTypes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllCategoryTypesQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });
            return Ok(result);
        }

        // Get category type by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryTypeById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryTypeByIdQuery(id));
            return Ok(result);
        }

        // Create a new category type
        [HttpPost]
        public async Task<IActionResult> CreateCategoryType([FromBody] CategoryTypeRequest categoryTypeRequest)
        {
            var result = await _mediator.Send(new CreateCategoryTypeCommand(categoryTypeRequest.Name, categoryTypeRequest.Description));
            return Ok(result);
        }

        // Update an existing category type
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryType(Guid id, [FromBody] CategoryTypeRequest categoryTypeRequest)
        {
            var result = await _mediator.Send(new UpdateCategoryTypeCommand(id, categoryTypeRequest.Name, categoryTypeRequest.Description));
            return Ok(result);
        }

        // Delete a category type
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryType(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryTypeCommand(id));
            return Ok(result);
        }
    }
}
