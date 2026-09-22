using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();

        public DateTime OrderDate { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Please enter the recipient's name")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Please enter a phone number")]
        [MaxLength(20)]
        [RegularExpression(@"^\+?[\d\s\-()]{6,20}$", ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please enter a country")]
        [MaxLength(100)]
        public string? Country { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Please enter a city")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Please enter a street")]
        [MaxLength(200)]
        public string? Street { get; set; }

        [Display(Name = "Building")]
        [Required(ErrorMessage = "Please enter a house number")]
        [MaxLength(50)]
        public string? Building { get; set; }

        [Display(Name = "Apartment")]
        [MaxLength(50)]
        public string? Apartment { get; set; }

        [Display(Name = "Postal code")]
        [MaxLength(10)]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Postal code must contain 5-6 digits")]
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

        public string? UserId { get; set; }
    }
}
