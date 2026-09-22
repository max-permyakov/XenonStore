using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Web.Models.ViewModels.Login;

namespace Xenon.Web.Controllers
{
    [Route("Account/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ILoggingService _loggingService;
        private readonly ICartService _cartService;
        private readonly IRecentlyViewedService _recentlyViewedService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ILoggingService loggingService,
            ICartService cartService,
            IRecentlyViewedService recentlyViewedService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _loggingService = loggingService;
            _cartService = cartService;
            _recentlyViewedService = recentlyViewedService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ViewResult Login(string? returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl ?? "/"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if (user == null)
                {
                    await _loggingService.LogWarningAsync("Auth",
                        $"Login attempt with unknown email: {loginModel.Email}",
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
                    ModelState.AddModelError("", "Неверный email или пароль");
                    return View(loginModel);
                }

                var sessionId = HttpContext.Session.Id;
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(
                    user, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    await _loggingService.LogInfoAsync("Auth",
                        $"User logged in: {user.Email}",
                        userId: user.Id, userName: user.UserName,
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

                    try
                    {
                        await _cartService.MigrateSessionCartToUserAsync(sessionId, user.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to migrate session cart for user {UserId}", user.Id);
                    }

                    try
                    {
                        var guestId = HttpContext.Request.Cookies["Xenon.FavoritesGuest"];
                        await _recentlyViewedService.MergeGuestToUserAsync(guestId, user.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to merge recently viewed for user {UserId}", user.Id);
                    }

                    if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Supplier"))
                        return Redirect(loginModel?.ReturnUrl ?? "/Admin");
                    return Redirect(loginModel?.ReturnUrl ?? "/");
                }
                if (result.IsLockedOut)
                {
                    await _loggingService.LogWarningAsync("Auth",
                        $"Account locked out: {user.Email}",
                        userId: user.Id, userName: user.UserName,
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
                    ModelState.AddModelError("", "Аккаунт заблокирован. Попробуйте позже.");
                }
                else
                {
                    await _loggingService.LogWarningAsync("Auth",
                        $"Failed login attempt for: {user.Email}",
                        userId: user.Id, userName: user.UserName,
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
                    ModelState.AddModelError("", "Неверный email или пароль");
                }
            }
            return View(loginModel);
        }

        [HttpGet]
        public ViewResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                var sessionId = HttpContext.Session.Id;
                await _signInManager.SignInAsync(user, isPersistent: false);

                try
                {
                    await _cartService.MigrateSessionCartToUserAsync(sessionId, user.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to migrate session cart after registration for {UserId}", user.Id);
                }

                try
                {
                    var guestId = HttpContext.Request.Cookies["Xenon.FavoritesGuest"];
                    await _recentlyViewedService.MergeGuestToUserAsync(guestId, user.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to merge recently viewed after registration for {UserId}", user.Id);
                }

                await _loggingService.LogInfoAsync("Auth",
                    $"New user registered: {user.Email}",
                    userId: user.Id, userName: user.UserName,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public ViewResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _loggingService.LogInfoAsync("Auth",
                    $"User logged out: {user.Email}",
                    userId: user.Id, userName: user.UserName,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
            }

            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
