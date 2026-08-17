using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Xenon.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        public long? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FullName => $"{FirstName} {LastName}";
    }
}
