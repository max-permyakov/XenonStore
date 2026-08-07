using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;
using Xenon.Infrastructure.Repositories;
using Xenon.Infrastructure.Services;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
    });
    builder.Services.AddControllersWithViews();
  
    builder.Services.Configure<SessionOptions>(options =>
    {
        options.Cookie.Name = ".XenenStore.Session";
        options.Cookie.IsEssential = true;
    });
    builder.Services.AddDbContext<StoreDbContext>(opts =>
    {
        opts.UseSqlServer(
            builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
      

    });
    builder.Services.AddDistributedSqlServerCache(opts =>
    {
        opts.ConnectionString
        = builder.Configuration["ConnectionStrings:CacheConnection"];
        opts.SchemaName = "dbo";
        opts.TableName = "DataCache";
    });
    builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\maksi\Documents\ScriptsOrUtilites\ProjectsC#\XenonStore\Web\App_Data\keys\")) 
    .SetApplicationName("XenonStore");
    builder.Services.AddDbContext<AppIdentityDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration["ConnectionStrings:IdentityConnection"]));
    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppIdentityDbContext>();
    //builder.Services.Configure<HostFilteringOptions>(opts => {
    //    opts.AllowedHosts.Clear();
    //    opts.AllowedHosts.Add("*.example.com");
    //});
    builder.Services.AddHsts(opts =>
    {
        opts.MaxAge = TimeSpan.FromDays(1);
        opts.IncludeSubDomains = true;
    });
    var servicesConfig = builder.Configuration;
    builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
    builder.Services.AddRazorPages();
    builder.Services.AddSession();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<ICartStorage, SessionCartStorage>();
    builder.Services.AddScoped<IFavoriteService, FavoriteService>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
    builder.Services.AddScoped<ProductImportService>();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Name = "XSRF-TOKEN";
        options.FormFieldName = "__RequestVerificationToken";
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(20);
        options.Cookie.IsEssential = true;
    });
    var app = builder.Build();

    if (app.Environment.IsProduction())
    {
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseCookiePolicy();
    app.UseCors();
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
    app.UseRequestLocalization(opts =>
    {
        opts.AddSupportedCultures("ru-Ru")
        .AddSupportedUICultures("ru-US")
        .SetDefaultCulture("en-US");
    });

    app.UseStaticFiles();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapRazorPages();
    app.MapBlazorHub();
    //app.MapFallbackToPage("/{*catchall}", "/error");
    //app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");
    SeedData.EnsurePopulated(app);
    IdentitySeedData.EnsurePopulated(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
