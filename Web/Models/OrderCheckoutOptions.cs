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
                    Name = "Курьером",
                    Description = "Доставим до двери в течение 1-3 дней",
                    Price = 300m,
                    FreeFrom = 3000m
                },
                new()
                {
                    Method = DeliveryMethod.Post,
                    Name = "Почтой России",
                    Description = "Отправка почтовым отправлением в отделение",
                    Price = 250m
                },
                new()
                {
                    Method = DeliveryMethod.PickupPoint,
                    Name = "Пункт выдачи / Самовывоз",
                    Description = "Бесплатно в пункте выдачи рядом с вами",
                    Price = 0m
                }
            };

        public static IReadOnlyList<PaymentOption> PaymentOptions { get; } =
            new List<PaymentOption>
            {
                new()
                {
                    Method = PaymentMethod.Sbp,
                    Name = "СБП (Система быстрых платежей)",
                    Description = "Оплата по QR-коду через приложение вашего банка"
                },
                new()
                {
                    Method = PaymentMethod.Card,
                    Name = "Банковская карта",
                    Description = "MIR, Visa, MasterCard — онлайн через защищённый шлюз"
                },
                new()
                {
                    Method = PaymentMethod.CashOnDelivery,
                    Name = "При получении",
                    Description = "Наличными или картой курьеру / в пункте выдачи"
                },
                new()
                {
                    Method = PaymentMethod.EWallet,
                    Name = "Электронный кошелёк",
                    Description = "ЮMoney и другие электронные кошельки"
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
            PaymentStatus.Pending => "Ожидает оплаты",
            PaymentStatus.Paid => "Оплачен",
            PaymentStatus.Failed => "Оплата не прошла",
            PaymentStatus.Refunded => "Возврат",
            _ => status.ToString()
        };
    }
}
