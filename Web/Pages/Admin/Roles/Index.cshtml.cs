using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Domain.Models;

namespace Xenon.Web.Pages.Admin.Roles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public List<IdentityRole> Roles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Roles = await Task.FromResult(_roleManager.Roles.ToList());
        }

        public async Task<string> GetMembersString(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            if (users.Count == 0) return "No members";
            var names = users.Take(3).Select(u => u.FullName).ToList();
            var result = string.Join(", ", names);
            return users.Count > 3 ? $"{result} (+{users.Count - 3} more)" : result;
        }

        public async Task<IActionResult> OnPostCreateAsync(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var role = new IdentityRole { Name = roleName.Trim() };
                await _roleManager.CreateAsync(role);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToPage();
        }
    }
}
