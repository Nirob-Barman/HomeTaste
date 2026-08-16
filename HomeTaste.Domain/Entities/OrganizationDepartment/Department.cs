namespace HomeTaste.Domain.Entities.OrganizationDepartment
{
    public class Department
    {
        public Guid Id { get; set; }
        public string? Name { get; private set; } // Name of the department (e.g., Kitchen, Delivery)
        public string? Description { get; private set; }

        private Department() { } // EF Core

        public static Department Create(string? name, string? description)
        {
            return new Department
            {
                Name = name,
                Description = description
            };
        }

        public void UpdateDetails(string? name, string? description)
        {
            Name = name ?? Name;
            Description = description ?? Description;
        }
    }
}
