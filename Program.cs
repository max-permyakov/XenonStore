using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SportsStore.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.HostFiltering;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<StoreDbContext>(opts => {
    opts.UseSqlServer(
        builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
});
//builder.Services.Configure<HostFilteringOptions>(opts => {
//    opts.AllowedHosts.Clear();
//    opts.AllowedHosts.Add("*.example.com");
//});
builder.Services.AddHsts(opts => {
    opts.MaxAge = TimeSpan.FromDays(1);
    opts.IncludeSubDomains = true;
});
var servicesConfig = builder.Configuration;
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();


builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IOrderRepository,EFOrderRepository>();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:IdentityConnection"]));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

var app = builder.Build();
//if (app.Environment.IsProduction())
//{
//    app.UseHsts();
//}
app.UseHttpsRedirection();
app.UseCookiePolicy();
var piplineConfig = app.Configuration;
app.MapGet("config", async (HttpContext context, IConfiguration config) =>
{
    string defaultDebug = config["Logging:LogLevel:Default"];
    await context.Response.WriteAsync($"The config is:{defaultDebug}");
    string environ = config["ASPNETCORE_ENVIRONMENT"];
    await context.Response.WriteAsync($"\nThe env setting is: {environ}");
    string wsID = config["WebService:Id"];
    string wsKey = config["WebService:Key"];
    await context.Response.WriteAsync($"\nThe secret ID is: {wsID}");
    await context.Response.WriteAsync($"\nThe secret Key is: {wsKey}");
    var conn = config["ConnectionStrings:SportsStoreConnection"];
    await context.Response.WriteAsync($"Connection: {conn ?? "NULL"}");
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
}
app.UseRequestLocalization(opts => {
    opts.AddSupportedCultures("ru-Ru")
    .AddSupportedUICultures("ru-US")
    .SetDefaultCulture("ru-Ru");
});

app.UseStaticFiles();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("catpage",
    "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("page", "Page{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute("pagination",
    "Products/Page{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/{*catchall}", "/error");
app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");
SeedData.EnsurePopulated(app);
IdentitySeedData.EnsurePopulated(app);

app.Run();