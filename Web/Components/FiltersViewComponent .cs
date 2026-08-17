using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Web.Components
{
    public class FiltersViewComponent : ViewComponent
    {
        private readonly IStoreRepository _repository;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FiltersViewComponent(
            IStoreRepository repository,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
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
            ViewBag.CartItemsCount = cart.Lines.Sum(l => l.Quantity);

            var categories = _repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);

            return View(categories);
        }
    }
}
