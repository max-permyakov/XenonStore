using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;


namespace Xenon.Infrastructure.Services
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminEmail = "admin@xenon.ru";
        private const string adminPassword = "SecurePass123$";

        private const string supplierUser = "SupplierDemo";
        private const string supplierEmail = "supplier@xenon.ru";
        private const string supplierPassword = "SecurePass123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            AppIdentityDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<AppIdentityDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            var serviceProvider = app.ApplicationServices
                .CreateScope().ServiceProvider;

            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Supplier", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            UserManager<ApplicationUser> userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByNameAsync(adminUser) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUser,
                    Email = adminEmail,
                    FirstName = "Администратор",
                    LastName = "Системный",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (await userManager.FindByNameAsync(supplierUser) == null)
            {
                var supplier = new ApplicationUser
                {
                    UserName = supplierUser,
                    Email = supplierEmail,
                    FirstName = "Демо",
                    LastName = "Поставщик",
                    EmailConfirmed = true,
                    SupplierId = 1
                };
                var result = await userManager.CreateAsync(supplier, supplierPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(supplier, "Supplier");
                }
            }
        }
    }
}
