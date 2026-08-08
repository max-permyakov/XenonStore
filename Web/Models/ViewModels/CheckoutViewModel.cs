using Xenon.Domain.Models;

namespace Xenon.Web.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; } = new Order();

        public Cart Cart { get; set; } = new Cart();

        public decimal Subtotal => Cart.ComputeTotalValue();

        public decimal ShippingCost { get; set; }

        public decimal Total => Subtotal + ShippingCost;

        public IReadOnlyList<DeliveryOption> DeliveryOptions
            => OrderCheckoutOptions.DeliveryOptions;

        public IReadOnlyList<PaymentOption> PaymentOptions
            => OrderCheckoutOptions.PaymentOptions;
    }
}
