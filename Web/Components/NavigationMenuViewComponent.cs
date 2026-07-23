using Microsoft.AspNetCore.Mvc;
namespace Xenon.Web.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View();
        }
    }
}