using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Measurements;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;
        public UnitsController(IUnitService unitService)
        {
            _unitService = unitService;
        }


        // Get all units
        [HttpGet]
        public async Task<IActionResult> GetAllUnits([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _unitService.GetAllUnitsAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get unit by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(Guid id)
        {
            var result = await _unitService.GetUnitByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new unit
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] UnitRequest unitRequest)
        {
            var result = await _unitService.CreateUnitAsync(unitRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing unit
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UnitRequest unitRequest)
        {
            //var result = await _unitService.UpdateUnitAsync(id, unitRequest);
            var result = await _unitService.UpdateUnitAsyncUsingDapperGetAndEfUpdate(id, unitRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _unitService.SoftDeleteUnitAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a unit
        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            var result = await _unitService.HardDeleteUnitAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }


        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedUnits()
        {
            var result = await _unitService.BulkInsertPredefinedUnitsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

    }
}