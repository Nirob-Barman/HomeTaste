using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.MealCustomizations;
using HomeTaste.Application.Features.MealCustomizations.Commands.CreateOption;
using HomeTaste.Application.Features.MealCustomizations.Commands.DeleteOption;
using HomeTaste.Application.Features.MealCustomizations.Commands.ToggleAvailability;
using HomeTaste.Application.Features.MealCustomizations.Commands.UpdateOption;
using HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionById;
using HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionsByMealId;
using MediatR;
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
        private readonly IMediator _mediator;

        public MealCustomizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets all customization options for a specific meal.</summary>
        [HttpGet("meal/{mealId:guid}")]
        public async Task<IActionResult> GetByMeal(Guid mealId)
        {
            var result = await _mediator.Send(new GetOptionsByMealIdQuery(mealId));
            return Ok(result);
        }

        /// <summary>Gets a single customization option by ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOptionByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Creates a new customization option for a meal. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] MealCustomizationOptionRequest request)
        {
            var result = await _mediator.Send(new CreateOptionCommand(request));
            return StatusCode(201, result);
        }

        /// <summary>Updates an existing customization option. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] MealCustomizationOptionRequest request)
        {
            var result = await _mediator.Send(new UpdateOptionCommand(id, request));
            return Ok(result);
        }

        /// <summary>Deletes a customization option. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteOptionCommand(id));
            return Ok(result);
        }

        /// <summary>Toggles the availability of a customization option. Admin only.</summary>
        [HttpPatch("{id:guid}/toggle-availability")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> ToggleAvailability(Guid id)
        {
            var result = await _mediator.Send(new ToggleAvailabilityCommand(id));
            return Ok(result);
        }
    }
}
