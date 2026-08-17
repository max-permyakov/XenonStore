using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin.Products
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

        public List<Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!isAdmin && user?.SupplierId != null)
            {
                query = query.Where(p => p.SupplierId == user.SupplierId);
            }

            Products = await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            if (!isAdmin && product.SupplierId != user?.SupplierId)
                return Forbid();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
