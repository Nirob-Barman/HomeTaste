using HomeTaste.Application.Features.Departments;
using HomeTaste.Application.Features.Departments.Commands.CreateDepartment;
using HomeTaste.Application.Features.Departments.Commands.DeleteDepartment;
using HomeTaste.Application.Features.Departments.Commands.UpdateDepartment;
using HomeTaste.Application.Features.Departments.Queries.GetAllDepartments;
using HomeTaste.Application.Features.Departments.Queries.GetDepartmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllDepartmentsQuery(pageNumber, pageSize, searchTerm));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(Guid id)
        {
            var result = await _mediator.Send(new GetDepartmentByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _mediator.Send(new CreateDepartmentCommand(departmentRequest.Name, departmentRequest.Description));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _mediator.Send(new UpdateDepartmentCommand(id, departmentRequest.Name, departmentRequest.Description));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var result = await _mediator.Send(new DeleteDepartmentCommand(id));
            return Ok(result);
        }
    }
}
