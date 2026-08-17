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
    public class EditModel : PageModel
    {
        private readonly StoreDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(StoreDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public Product Product { get; set; } = null!;

        public SelectList CategoryOptions { get; set; } = null!;
        public SelectList SupplierOptions { get; set; } = null!;
        public bool IsSupplierReadOnly { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && product.SupplierId != user?.SupplierId)
                return Forbid();

            Product = product;
            await LoadOptionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync();
                return Page();
            }

            var existing = await _db.Products.FindAsync(Product.ProductID);
            if (existing == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && existing.SupplierId != user?.SupplierId)
                return Forbid();

            existing.Name = Product.Name;
            existing.Description = Product.Description;
            existing.Price = Product.Price;
            existing.CategoryId = Product.CategoryId;
            existing.SupplierId = Product.SupplierId;
            existing.Discount = Product.Discount;
            existing.Rating = Product.Rating;
            existing.Currency = Product.Currency;
            existing.Features = Product.Features;
            existing.ImageUrl = Product.ImageUrl;

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
                var supplier = await _db.Suppliers.FindAsync(user.SupplierId);
                SupplierName = supplier?.Name ?? "";
            }
        }
    }
}
