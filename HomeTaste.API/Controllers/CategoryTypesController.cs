using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces.Support;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryTypesController : ControllerBase
    {
        private readonly ICategoryTypeService _categoryTypeService;

        public CategoryTypesController(ICategoryTypeService categoryTypeService)
        {
            _categoryTypeService = categoryTypeService;
        }

        // Get all category types with pagination and search
        [HttpGet]
        public async Task<IActionResult> GetAllCategoryTypes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _categoryTypeService.GetAllCategoryTypesAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get category type by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryTypeById(Guid id)
        {
            var result = await _categoryTypeService.GetCategoryTypeByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new category type
        [HttpPost]
        public async Task<IActionResult> CreateCategoryType([FromBody] CategoryTypeRequest categoryTypeRequest)
        {
            var result = await _categoryTypeService.CreateCategoryTypeAsync(categoryTypeRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing category type
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryType(Guid id, [FromBody] CategoryTypeRequest categoryTypeRequest)
        {
            var result = await _categoryTypeService.UpdateCategoryTypeAsync(id, categoryTypeRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a category type
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryType(Guid id)
        {
            var result = await _categoryTypeService.DeleteCategoryTypeAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
