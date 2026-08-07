using Microsoft.AspNetCore.Mvc;
using Xenon.Domain.Interfaces.Services;
using Xenon.Web.Models;

namespace Xenon.Web.Components
{
    public class FavoritesSummaryViewComponent : ViewComponent
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesSummaryViewComponent(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var owner = FavoriteOwner.Resolve(HttpContext.User, HttpContext);
            var count = await _favoriteService.GetCountAsync(owner.UserId, owner.SessionId);
            return View(count);
        }
    }
}
