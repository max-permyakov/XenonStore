using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Xenon.Web.Models
{
    public static class FavoriteOwner
    {
        private const string GuestCookieName = "Xenon.FavoritesGuest";

        public static (string? UserId, string? SessionId) Resolve(
            ClaimsPrincipal? user,
            HttpContext? httpContext)
        {
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                    return (userId, null);
            }

            if (httpContext != null)
            {
                var existing = httpContext.Request.Cookies[GuestCookieName];
                if (!string.IsNullOrEmpty(existing))
                    return (null, existing);

                var guestId = Guid.NewGuid().ToString("N");
                httpContext.Response.Cookies.Append(GuestCookieName, guestId, new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                });
                return (null, guestId);
            }

            return (null, httpContext?.Session?.Id);
        }
    }
}
