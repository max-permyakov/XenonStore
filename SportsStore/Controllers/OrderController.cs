using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Models;

namespace Xenon.Web.Controllers

{
    [Route("Order/[action]")]
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private readonly ILogger<OrderController> _logger;
        
        public OrderController(IOrderRepository repoService, Cart cartService, ILogger<OrderController> logger)
        {
            repository = repoService;
            cart = cartService;
            _logger = logger;
        }
        [HttpGet]
        public ViewResult Checkout() => View(new Order());
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.Lines.ToArray();
                repository.SaveOrder(order);
                cart.Clear();
                _logger.LogInformation($"New Order by {order.Name}");
                return RedirectToPage("/Completed", new { orderId = order.OrderID });
            }
            else
            {
                return View();
            }
        }
    }
}