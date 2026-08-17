using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin
{
    [Authorize(Roles = "Admin,Supplier")]
    public class IndexModel : PageModel
    {
        private readonly StoreDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(StoreDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int UnreadNotifications { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public List<Notification> RecentNotifications { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            TotalProducts = isAdmin
                ? await _db.Products.CountAsync()
                : await _db.Products.CountAsync(p => p.SupplierId == user!.SupplierId);

            TotalOrders = isAdmin
                ? await _db.Orders.CountAsync()
                : await _db.Orders.Where(o => o.Lines.Any(l => l.Product.SupplierId == user!.SupplierId)).CountAsync();

            TotalUsers = await _userManager.Users.CountAsync();
            UnreadNotifications = await _db.Notifications.CountAsync(n => !n.IsRead);

            RecentOrders = isAdmin
                ? await _db.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync()
                : await _db.Orders
                    .Where(o => o.Lines.Any(l => l.Product.SupplierId == user!.SupplierId))
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync();

            RecentNotifications = await _db.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
