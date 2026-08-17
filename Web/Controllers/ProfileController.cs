using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;

namespace Xenon.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILoggingService _loggingService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggingService loggingService,
            IOrderRepository orderRepository,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _loggingService = loggingService;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var userId = user.Id;
            var orders = _orderRepository.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.RecentOrders = orders.Take(5).ToList();
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalSpent = orders.Sum(o => o.TotalAmount);

            return View(user);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.Country = model.Country;
            user.PostalCode = model.PostalCode;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _loggingService.LogInfoAsync("Profile",
                    $"Profile updated: {user.Email}",
                    userId: user.Id, userName: user.UserName,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

                TempData["Success"] = "Профиль обновлён";
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        public IActionResult Security() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Security(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Все поля обязательны");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                await _loggingService.LogInfoAsync("Security",
                    $"Password changed: {user.Email}",
                    userId: user.Id, userName: user.UserName,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

                TempData["Success"] = "Пароль изменён";
                return RedirectToAction(nameof(Security));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var orders = _orderRepository.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (avatar != null && avatar.Length > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(avatar.ContentType))
                {
                    TempData["Error"] = "Допустимые форматы: JPEG, PNG, WebP";
                    return RedirectToAction(nameof(Settings));
                }

                if (avatar.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "Максимальный размер файла: 5MB";
                    return RedirectToAction(nameof(Settings));
                }

                var ext = Path.GetExtension(avatar.FileName);
                var fileName = $"{user.Id}{ext}";
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                user.AvatarUrl = $"/images/avatars/{fileName}";
                await _userManager.UpdateAsync(user);

                await _loggingService.LogInfoAsync("Profile",
                    $"Avatar uploaded: {user.Email}",
                    userId: user.Id, userName: user.UserName);
            }

            return RedirectToAction(nameof(Settings));
        }
    }
}
