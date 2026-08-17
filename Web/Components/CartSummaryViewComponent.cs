using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Web.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartSummaryViewComponent(
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCartId()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                    return userId;
            }
            return _httpContextAccessor.HttpContext?.Session?.Id ?? Guid.NewGuid().ToString();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await _cartService.GetCartAsync(GetCartId());
            return View(cart);
        }
    }
}
