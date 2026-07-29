using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Supplier
    {
        public long SupplierId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public IEnumerable<Product>? Products { get; set; }
    }
}