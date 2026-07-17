using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Infrastructure;
namespace SportsStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View();
        }
    }
}