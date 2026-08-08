using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Domain.Interfaces;
using Xenon.Domain.Models;

namespace Xenon.Web.Pages
{
    public class CompletedModel : PageModel
    {
        private readonly IOrderRepository _repository;

        public CompletedModel(IOrderRepository repository)
        {
            _repository = repository;
        }

        public Order? Order { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? OrderId { get; set; }

        public void OnGet()
        {
            if (OrderId.HasValue)
            {
                Order = _repository.Orders.FirstOrDefault(o => o.OrderID == OrderId.Value);
            }
        }
    }
}
