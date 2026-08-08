using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models;
using Xenon.Web.Models.ViewModels;

namespace Xenon.Web.Controllers

{
    [Route("Order/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository repository;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<OrderController> _logger;

        private string CartId => _httpContextAccessor.HttpContext?.Session?.Id
            ?? Guid.NewGuid().ToString();

        public OrderController(IOrderRepository repoService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            ILogger<OrderController> logger)
        {
            repository = repoService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartService.GetCartAsync(CartId);
            if (cart.Lines.Count == 0)
            {
                return RedirectToPage("/Cart");
            }

            var order = new Order
            {
                DeliveryMethod = DeliveryMethod.Courier,
                PaymentMethod = PaymentMethod.Sbp
            };
            await PrefillFromUserAsync(order);

            return View(BuildViewModel(order, cart));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = await _cartService.GetCartAsync(CartId);
            if (cart.Lines.Count == 0)
            {
                ModelState.AddModelError("", "Ваша корзина пуста!");
            }
            if (!ModelState.IsValid)
            {
                return View(BuildViewModel(order, cart));
            }

            var subtotal = cart.ComputeTotalValue();
            order.OrderDate = DateTime.UtcNow;
            order.ShippingCost = OrderCheckoutOptions.GetShippingCost(order.DeliveryMethod, subtotal);
            order.TotalAmount = subtotal + order.ShippingCost;
            order.PaymentStatus = PaymentStatus.Pending;
            order.Shipped = false;
            order.Lines = cart.Lines.ToArray();

            repository.SaveOrder(order);
            await _cartService.ClearCartAsync(CartId);
            _logger.LogInformation($"New order #{order.OrderID} by {order.Name}");

            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        }

        private CheckoutViewModel BuildViewModel(Order order, Cart cart)
        {
            return new CheckoutViewModel
            {
                Order = order,
                Cart = cart,
                ShippingCost = OrderCheckoutOptions.GetShippingCost(
                    order.DeliveryMethod, cart.ComputeTotalValue())
            };
        }

        private async Task PrefillFromUserAsync(Order order)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        order.Name ??= user.UserName;
                        order.Email ??= user.Email;
                    }
                }
            }
        }
    }
}
