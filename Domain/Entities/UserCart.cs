using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class UserCart
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();
    }
}
