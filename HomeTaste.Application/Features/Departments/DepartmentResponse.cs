namespace HomeTaste.Application.Features.Departments
{
    public record DepartmentResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
