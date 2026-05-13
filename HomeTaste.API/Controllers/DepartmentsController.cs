using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.OrganizationDepartment;
using HomeTaste.Application.Interfaces.OrganizationDepartment;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _departmentService.GetAllDepartmentsAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(Guid id)
        {
            var result = await _departmentService.GetDepartmentByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _departmentService.CreateDepartmentAsync(departmentRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _departmentService.UpdateDepartmentAsync(id, departmentRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
