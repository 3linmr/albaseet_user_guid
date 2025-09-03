using System.Text.Json;
using Shared.Helper.Identity;

namespace Shared.API.Helpers
{
    public static class SessionHelper
    {
        public static string? GetConnectionString()
        {
            var builder = WebApplication.CreateBuilder();
            return builder.Configuration.GetConnectionString("RESContext");
        }
        public static string? GetAdminLink()
        {
            var builder = WebApplication.CreateBuilder();
            return builder.Configuration.GetSection("Admin").Value;
        }

        public static async Task<bool> InitializeSessions(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext?.Session.GetString("UserName") != null) return true;
            if (httpContextAccessor != null)
            {
                var userId = httpContextAccessor.GetUserId();
                var userName = await httpContextAccessor.GetUserName();

                httpContextAccessor?.HttpContext?.Session.SetString("UserName", userName ?? "");
                httpContextAccessor?.HttpContext?.Session.SetString("UserId", userId ?? "");

                var menusFromUserDetail = httpContextAccessor?.GetUserMenus();

                if (menusFromUserDetail != null)
                {
                    var menuJson = JsonSerializer.Serialize(menusFromUserDetail);
                    httpContextAccessor?.HttpContext?.Session.SetString("menus", menuJson);
                }
            }
            return true;
		}
    }
}
