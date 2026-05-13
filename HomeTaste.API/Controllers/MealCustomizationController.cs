using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Manages customization options (add-ons, removals, substitutions) for meals.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MealCustomizationController : ControllerBase
    {
        private readonly IMealCustomizationService _customizationService;

        public MealCustomizationController(IMealCustomizationService customizationService)
        {
            _customizationService = customizationService;
        }

        /// <summary>Gets all customization options for a specific meal.</summary>
        [HttpGet("meal/{mealId:guid}")]
        public async Task<IActionResult> GetByMeal(Guid mealId)
        {
            var result = await _customizationService.GetOptionsByMealIdAsync(mealId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a single customization option by ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _customizationService.GetOptionByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Creates a new customization option for a meal. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] MealCustomizationOptionRequest request)
        {
            var result = await _customizationService.CreateOptionAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates an existing customization option. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] MealCustomizationOptionRequest request)
        {
            var result = await _customizationService.UpdateOptionAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes a customization option. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _customizationService.DeleteOptionAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Toggles the availability of a customization option. Admin only.</summary>
        [HttpPatch("{id:guid}/toggle-availability")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> ToggleAvailability(Guid id)
        {
            var result = await _customizationService.ToggleAvailabilityAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
