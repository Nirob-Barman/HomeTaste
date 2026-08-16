namespace HomeTaste.Application.Features.Departments
{
    public record DepartmentRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
