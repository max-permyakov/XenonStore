namespace Xenon.Domain.Models
{
    public enum DeliveryMethod
    {
        Courier = 1,
        Post,
        PickupPoint
    }

    public enum PaymentMethod
    {
        Sbp = 1,
        Card,
        CashOnDelivery,
        EWallet
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Paid,
        Failed,
        Refunded
    }
}
