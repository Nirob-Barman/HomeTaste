
namespace HomeTaste.Application.DTOs.Admin
{
    public class CreateRoleRequest
    {
        public string? RoleName { get; set; }
    }
    public class RoleActionResponse
    {
        public string? RoleName { get; set; }
    }
    public class DeleteRoleRequest
    {
        public string? RoleName { get; set; }
    }
}
