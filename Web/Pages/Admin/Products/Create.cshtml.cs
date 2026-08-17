using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin.Products
{
    [Authorize(Roles = "Admin,Supplier")]
    public class CreateModel : PageModel
    {
        private readonly StoreDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(StoreDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        public SelectList CategoryOptions { get; set; } = null!;
        public SelectList SupplierOptions { get; set; } = null!;
        public bool IsSupplierReadOnly { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadOptionsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync();
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && user?.SupplierId != null)
            {
                Product.SupplierId = user.SupplierId.Value;
            }

            Product.Popularity = 0;
            _db.Products.Add(Product);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadOptionsAsync()
        {
            CategoryOptions = new SelectList(await _db.Categories.ToListAsync(), "CategoryId", "Name");
            SupplierOptions = new SelectList(await _db.Suppliers.ToListAsync(), "SupplierId", "Name");

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && user?.SupplierId != null)
            {
                IsSupplierReadOnly = true;
                Product.SupplierId = user.SupplierId.Value;
                var supplier = await _db.Suppliers.FindAsync(user.SupplierId);
                SupplierName = supplier?.Name ?? "";
            }
        }
    }
}
