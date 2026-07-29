using System.ComponentModel.DataAnnotations;

namespace Xenon.Domain.Models
{
    public class Product
    {
        public long ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public long CategoryId { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        public long SupplierId { get; set; }
        public decimal? Discount { get; set; }  // Скидка в процентах (0.1 = 10%)
        public double? Rating { get; set; }     // Рейтинг (например, 4.5)
        public string? Currency { get; set; }   // Валюта (USD, EUR и т.д.)
        public string? Features { get; set; }   // Особенности (может быть длинный текст)
        public string? ImageUrl { get; set; }   // Ссылка на изображение (добавим позже)
        // Навигационные свойства
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
    }
}