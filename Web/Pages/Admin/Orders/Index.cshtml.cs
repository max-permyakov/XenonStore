using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin.Orders
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

        public List<Order> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            var query = _db.Orders
                .Include(o => o.Lines).ThenInclude(l => l.Product)
                .AsQueryable();

            if (!isAdmin && user?.SupplierId != null)
            {
                query = query.Where(o => o.Lines.Any(l => l.Product.SupplierId == user.SupplierId));
            }

            Orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleShipAsync(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin"))
            {
                var hasProduct = await _db.Orders
                    .Where(o => o.OrderID == id)
                    .AnyAsync(o => o.Lines.Any(l => l.Product.SupplierId == user!.SupplierId));
                if (!hasProduct) return Forbid();
            }

            order.Shipped = !order.Shipped;
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
