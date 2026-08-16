namespace HomeTaste.Application.Features.Users.Admin
{
    public record AssignRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public record RoleAssignmentResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public record RemoveRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }

    public record RoleRemovalResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
}
