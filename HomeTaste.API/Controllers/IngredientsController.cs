using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
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
            var result = await _ingredientService.GetAllIngredientsAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get ingredient by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredientById(Guid id)
        {
            var result = await _ingredientService.GetIngredientByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
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
            var result = await _ingredientService.CreateIngredientAsync(ingredientRequest, fileDto);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] IngredientRequest ingredientRequest)
        {
            var result = await _ingredientService.UpdateIngredientAsync(id, ingredientRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete an ingredient
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(Guid id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedIngredients()
        {
            var result = await _ingredientService.BulkInsertPredefinedIngredientsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
