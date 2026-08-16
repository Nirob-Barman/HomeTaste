namespace HomeTaste.Application.Features.Users.Admin
{
    public class AssignRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public class RoleAssignmentResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public class RemoveRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public class RoleRemovalResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
}
