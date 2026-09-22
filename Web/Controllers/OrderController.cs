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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderController> _logger;
        private readonly ILoggingService _loggingService;

        private string GetCartId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                    return userId;
            }
            return _httpContextAccessor.HttpContext?.Session?.Id ?? Guid.NewGuid().ToString();
        }

        public OrderController(
            IOrderRepository repoService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            ILogger<OrderController> logger,
            ILoggingService loggingService)
        {
            repository = repoService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartService.GetCartAsync(GetCartId());
            if (cart.Lines.Count == 0)
            {
                return RedirectToPage("/Cart");
            }

            var order = new Order
            {
                DeliveryMethod = DeliveryMethod.Courier,
                PaymentMethod = PaymentMethod.Sbp
            };

            ApplicationUser? userProfile = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    userProfile = await _userManager.FindByIdAsync(userId);
                    if (userProfile != null)
                    {
                        order.Name = userProfile.FullName;
                        order.Email = userProfile.Email;
                        order.Phone = userProfile.PhoneNumber;
                        order.Country = userProfile.Country;
                        order.City = userProfile.City;
                        order.Street = userProfile.Address;
                        order.UserId = userId;
                    }
                }
            }

            return View(BuildViewModel(order, cart, userProfile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = await _cartService.GetCartAsync(GetCartId());
            if (cart.Lines.Count == 0)
            {
                ModelState.AddModelError("", "Корзина пуста!");
            }

            ApplicationUser? userProfile = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    userProfile = await _userManager.FindByIdAsync(userId);
                    order.UserId = userId;
                }
            }

            if (!ModelState.IsValid)
            {
                return View(BuildViewModel(order, cart, userProfile));
            }

            var subtotal = cart.ComputeTotalValue();
            order.OrderDate = DateTime.UtcNow;
            order.ShippingCost = OrderCheckoutOptions.GetShippingCost(order.DeliveryMethod, subtotal);
            order.TotalAmount = subtotal + order.ShippingCost;
            order.PaymentStatus = PaymentStatus.Pending;
            order.Shipped = false;
            order.Lines = cart.Lines
                .Select(l => new CartLine { Product = l.Product, Quantity = l.Quantity })
                .ToList();

            repository.SaveOrder(order);
            await _cartService.ClearCartAsync(GetCartId());

            await _loggingService.LogInfoAsync("Order",
                $"New order #{order.OrderID}: {order.TotalAmount:N2} by {order.Name}",
                userId: order.UserId, userName: order.Name,
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                entityType: "Order", entityId: order.OrderID);

            if (order.TotalAmount >= 10000m)
            {
                await _loggingService.LogWarningAsync("Order",
                    $"Large order #{order.OrderID}: {order.TotalAmount:N2} by {order.Name}",
                    userId: order.UserId, userName: order.Name,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    entityType: "Order", entityId: order.OrderID,
                    metadata: $"{{\"amount\":{order.TotalAmount},\"items\":{cart.Lines.Count}}}");

                await _loggingService.NotifyAsync(
                    "Крупный заказ",
                    $"Заказ #{order.OrderID} на сумму {order.TotalAmount:N2}₽ от {order.Name}",
                    "Warning",
                    $"/Admin/Orders/Details/{order.OrderID}");
            }

            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        }

        private CheckoutViewModel BuildViewModel(Order order, Cart cart, ApplicationUser? userProfile = null)
        {
            return new CheckoutViewModel
            {
                Order = order,
                Cart = cart,
                ShippingCost = OrderCheckoutOptions.GetShippingCost(
                    order.DeliveryMethod, cart.ComputeTotalValue()),
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                UserProfile = userProfile
            };
        }
    }
}
