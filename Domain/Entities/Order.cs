using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();

        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Укажите имя получателя")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Укажите телефон для связи")]
        [MaxLength(20)]
        [RegularExpression(@"^\+?[\d\s\-()]{6,20}$", ErrorMessage = "Укажите корректный номер телефона")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Укажите email")]
        [EmailAddress(ErrorMessage = "Укажите корректный email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Укажите страну")]
        [MaxLength(100)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Required(ErrorMessage = "Укажите улицу")]
        [MaxLength(200)]
        public string? Street { get; set; }

        [Required(ErrorMessage = "Укажите номер дома")]
        [MaxLength(50)]
        public string? Building { get; set; }

        [MaxLength(50)]
        public string? Apartment { get; set; }

        [Required(ErrorMessage = "Укажите почтовый индекс")]
        [MaxLength(10)]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Индекс должен содержать 5-6 цифр")]
        public string? PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? PaymentId { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal TotalAmount { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool Shipped { get; set; }
    }
}
