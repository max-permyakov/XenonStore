// Xenon.Infrastructure/Services/ProductImportService.cs
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;
using Xenon.Application.DTOs;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

public class ProductImportService
{
    private readonly StoreDbContext _context;
    private readonly ILogger<ProductImportService> _logger;

    public ProductImportService(StoreDbContext context, ILogger<ProductImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> ImportFromCsvAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Если в CSV нет заголовков, используйте:
        // csv.Configuration.HasHeaderRecord = false;
        // и тогда маппинг по индексам.

        var records = csv.GetRecords<ProductImportDto>().ToList();
        _logger.LogInformation($"Loaded {records.Count} records from CSV");

        var categories = new Dictionary<string, Category>();

        foreach (var record in records)
        {
            // 1. Парсим цену (убираем валюту)
            var price = ParsePrice(record.PriceString);

            // 2. Парсим рейтинг (извлекаем число)
            var rating = ParseRating(record.RatingString);

            // 3. Скидку сохраняем как текст (если нужно)
            var discountText = record.DiscountString;

            // 4. Валюта — берем из цены или из поля Currency
            var currency = string.IsNullOrWhiteSpace(record.CurrencyString)
                ? "USD"
                : record.CurrencyString;

            // 5. Получаем категорию
            var category = await GetOrCreateCategory(record.SubCategory, categories);

            // 6. Проверяем дубликаты
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == record.Title);

            if (existing != null)
            {
                _logger.LogDebug($"Product '{record.Title}' already exists, skipping");
                continue;
            }
            var defaultSupplier = await GetOrCreateDefaultSupplier();
            // 7. Создаем товар
            var product = new Product
            {
                Name = record.Title,
                Description = record.Feature ?? string.Empty,
                Price = price,
                CategoryId = category.CategoryId,
                SupplierId = defaultSupplier.SupplierId,
                Discount = null, // не используем числовую скидку
                Rating = rating,
                Currency = currency,
                Features = record.Feature,
                ImageUrl = GenerateImageUrl(record.Title)
            };

            _context.Products.Add(product);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Imported {records.Count} products");

        return records.Count;
    }

    private decimal ParsePrice(string priceString)
    {
        if (string.IsNullOrWhiteSpace(priceString))
            return 0;

        // Убираем все символы, кроме цифр, точки и запятой
        var cleaned = Regex.Replace(priceString, @"[^\d.,]", "");
        // Если есть запятая как разделитель тысяч, заменяем на пустую
        cleaned = cleaned.Replace(",", "");
        // Если есть точка как разделитель тысяч, заменяем на пустую (зависит от локали)
        // В нашем случае цена $169.99 — точка разделитель десятичных
        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }

    private double? ParseRating(string ratingString)
    {
        if (string.IsNullOrWhiteSpace(ratingString))
            return null;

        // Ищем число с точкой или запятой (например, 4.4)
        var match = Regex.Match(ratingString, @"(\d+\.?\d*)");
        if (match.Success && double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    private async Task<Category> GetOrCreateCategory(string categoryName, Dictionary<string, Category> cache)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            categoryName = "Uncategorized";

        if (cache.TryGetValue(categoryName, out var category))
            return category;

        category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
        {
            category = new Category { Name = categoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(); // чтобы получить Id
        }

        cache[categoryName] = category;
        return category;
    }

    private string GenerateImageUrl(string title)
    {
        var seed = Math.Abs(title.GetHashCode());
        return $"https://picsum.photos/seed/{seed}/300/300";
    }
    private async Task<Supplier> GetOrCreateDefaultSupplier()
    {
        const string defaultName = "Unknown Supplier";
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == defaultName);
        if (supplier == null)
        {
            supplier = new Supplier { Name = defaultName, City = "N/A" };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }
        return supplier;
    }
}