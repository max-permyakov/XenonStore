using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Web.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly StoreDbContext _db;

        public IndexModel(StoreDbContext db)
        {
            _db = db;
        }

        public List<Category> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _db.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var category = new Category { Name = name.Trim() };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
