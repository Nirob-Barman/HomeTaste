using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Features.Units.Commands.BulkInsertUnits;
using HomeTaste.Application.Features.Units.Commands.CreateUnit;
using HomeTaste.Application.Features.Units.Commands.HardDeleteUnit;
using HomeTaste.Application.Features.Units.Commands.SoftDeleteUnit;
using HomeTaste.Application.Features.Units.Commands.UpdateUnit;
using HomeTaste.Application.Features.Units.Queries.GetAllUnits;
using HomeTaste.Application.Features.Units.Queries.GetUnitById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UnitsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // Get all units
        [HttpGet]
        public async Task<IActionResult> GetAllUnits([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllUnitsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get unit by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(Guid id)
        {
            var result = await _mediator.Send(new GetUnitByIdQuery(id));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new unit
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] UnitRequest unitRequest)
        {
            var result = await _mediator.Send(new CreateUnitCommand(unitRequest));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing unit
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UnitRequest unitRequest)
        {
            var result = await _mediator.Send(new UpdateUnitCommand(id, unitRequest));
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _mediator.Send(new SoftDeleteUnitCommand(id));
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a unit
        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            var result = await _mediator.Send(new HardDeleteUnitCommand(id));
            return ApiResponseMapper.FromResult(this, result);
        }


        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedUnits()
        {
            var result = await _mediator.Send(new BulkInsertUnitsCommand());
            return ApiResponseMapper.FromResult(this, result);
        }

    }
}