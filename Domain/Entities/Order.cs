using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();

        [Required(ErrorMessage = "Please enter a name")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter the first address line")]
        [MaxLength(200)]
        public string? Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [MaxLength(200)]
        public string? Line3 { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter a state name")]
        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        [MaxLength(100)]
        public string? Country { get; set; }

        public bool GiftWrap { get; set; }

        public bool Shipped { get; set; }
    }
}