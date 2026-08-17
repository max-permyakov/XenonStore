using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin.Suppliers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly StoreDbContext _db;

        public IndexModel(StoreDbContext db)
        {
            _db = db;
        }

        public List<Supplier> Suppliers { get; set; } = new();

        public async Task OnGetAsync()
        {
            Suppliers = await _db.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync(string name, string city)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var supplier = new Supplier
                {
                    Name = name.Trim(),
                    City = city?.Trim() ?? ""
                };
                _db.Suppliers.Add(supplier);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _db.Suppliers.Remove(supplier);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
