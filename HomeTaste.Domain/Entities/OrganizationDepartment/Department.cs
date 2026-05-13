namespace HomeTaste.Domain.Entities.OrganizationDepartment
{
    public class Department
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } // Name of the department (e.g., Kitchen, Delivery)
        public string? Description { get; set; }
    }
}
