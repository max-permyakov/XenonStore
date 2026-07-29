using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Category
    {
        public long CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Product>? Products { get; set; }
    }
}