using CsvHelper;
using Microsoft.AspNetCore.Hosting;
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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductImportService(
    StoreDbContext context,
    ILogger<ProductImportService> logger,
    IWebHostEnvironment webHostEnvironment) 
    {
        _context = context;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
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
            var price = ParsePrice(record.PriceString);

            var rating = ParseRating(record.RatingString);

            var discountText = record.DiscountString;

            var currency = string.IsNullOrWhiteSpace(record.CurrencyString)
                ? "USD"
                : record.CurrencyString;

            
            var category = await GetOrCreateCategory(record.SubCategory, categories);

         
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == record.Title);

            if (existing != null)
            {
                _logger.LogDebug($"Product '{record.Title}' already exists, skipping");
                continue;
            }
            var defaultSupplier = await GetOrCreateDefaultSupplier();
            var product = new Product
            {
                Name = record.Title,
                Description = record.Feature ?? string.Empty,
                Price = price,
                CategoryId = category.CategoryId,
                SupplierId = defaultSupplier.SupplierId,
                Discount = null,
                Rating = rating,
                Currency = currency,
                Features = record.Feature,
                ImageUrl = GetImageUrlForCategory(record.SubCategory, record.Title)
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

        var cleaned = Regex.Replace(priceString, @"[^\d.,]", "");
        cleaned = cleaned.Replace(",", "");
        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }

    private double? ParseRating(string ratingString)
    {
        if (string.IsNullOrWhiteSpace(ratingString))
            return null;

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
            await _context.SaveChangesAsync(); 
        }

        cache[categoryName] = category;
        return category;
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
    private readonly Dictionary<string, int> _imageIndexPerCategory = new();

    // Метод для получения URL изображения
    private string GetImageUrlForCategory(string categoryName, string productName)
    {
        // Нормализуем имя категории (убираем пробелы, спецсимволы)
        var normalizedCategory = categoryName?.Trim() ?? "Uncategorized";
        // Путь к папке с изображениями для этой категории
        var categoryFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", normalizedCategory);

        if (!Directory.Exists(categoryFolder))
        {
            // Если папки нет — используем общую папку или изображение по умолчанию
            categoryFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "common");
            if (!Directory.Exists(categoryFolder))
            {
                // Если и общей нет — возвращаем заглушку
                return "/images/placeholder.jpg";
            }
        }

      
        var imageFiles = Directory.GetFiles(categoryFolder)
            .Where(f => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                .Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        if (!imageFiles.Any())
        {
           
            return "/images/placeholder.jpg";
        }

  
        if (!_imageIndexPerCategory.ContainsKey(normalizedCategory))
        {
            _imageIndexPerCategory[normalizedCategory] = 0;
        }

       
        var index = _imageIndexPerCategory[normalizedCategory] % imageFiles.Count;
        var selectedFile = imageFiles[index];

      
        _imageIndexPerCategory[normalizedCategory]++;
        var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, selectedFile);
        return "/" + relativePath.Replace('\\', '/');
    }
}