using Xenon.Domain.Models;

namespace Xenon.Web.Models
{
    public class DeliveryOption
    {
        public DeliveryMethod Method { get; init; }
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public decimal Price { get; init; }
        public decimal? FreeFrom { get; init; }
    }

    public class PaymentOption
    {
        public PaymentMethod Method { get; init; }
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
    }

    public static class OrderCheckoutOptions
    {
        public static IReadOnlyList<DeliveryOption> DeliveryOptions { get; } =
            new List<DeliveryOption>
            {
                new()
                {
                    Method = DeliveryMethod.Courier,
                    Name = "Courier",
                    Description = "Delivery to your door within 1-3 days",
                    Price = 300m,
                    FreeFrom = 3000m
                },
                new()
                {
                    Method = DeliveryMethod.Post,
                    Name = "Postal service",
                    Description = "Shipped to your local post office",
                    Price = 250m
                },
                new()
                {
                    Method = DeliveryMethod.PickupPoint,
                    Name = "Pickup point",
                    Description = "Free pickup at a pickup point near you",
                    Price = 0m
                }
            };

        public static IReadOnlyList<PaymentOption> PaymentOptions { get; } =
            new List<PaymentOption>
            {
                new()
                {
                    Method = PaymentMethod.Sbp,
                    Name = "SBP (Fast Payment System)",
                    Description = "Pay by QR code through your bank's app"
                },
                new()
                {
                    Method = PaymentMethod.Card,
                    Name = "Bank card",
                    Description = "MIR, Visa, MasterCard — pay online via a secure gateway"
                },
                new()
                {
                    Method = PaymentMethod.CashOnDelivery,
                    Name = "Cash on delivery",
                    Description = "Pay in cash or by card on delivery / at the pickup point"
                },
                new()
                {
                    Method = PaymentMethod.EWallet,
                    Name = "E-wallet",
                    Description = "YooMoney and other e-wallets"
                }
            };

        public static decimal GetShippingCost(DeliveryMethod method, decimal subtotal)
        {
            var option = DeliveryOptions.First(o => o.Method == method);
            return option.Price > 0 && option.FreeFrom.HasValue && subtotal >= option.FreeFrom.Value
                ? 0m
                : option.Price;
        }

        public static string PaymentMethodName(PaymentMethod method)
            => PaymentOptions.First(o => o.Method == method).Name;

        public static string PaymentStatusName(PaymentStatus status) => status switch
        {
            PaymentStatus.Pending => "Pending payment",
            PaymentStatus.Paid => "Paid",
            PaymentStatus.Failed => "Payment failed",
            PaymentStatus.Refunded => "Refunded",
            _ => status.ToString()
        };
    }
}
