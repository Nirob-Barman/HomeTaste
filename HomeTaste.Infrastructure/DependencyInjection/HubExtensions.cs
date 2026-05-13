using HomeTaste.Infrastructure.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace HomeTaste.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Maps SignalR hubs so the API layer doesn't need to reference Infrastructure hub types directly.
    /// Call app.MapNotificationHub() in Program.cs after app.UseAuthorization().
    /// </summary>
    public static class HubExtensions
    {
        public static IEndpointRouteBuilder MapNotificationHub(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>("/hubs/notifications");
            return endpoints;
        }
    }
}
