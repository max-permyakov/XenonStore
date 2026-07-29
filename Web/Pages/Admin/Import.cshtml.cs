using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xenon.Infrastructure.Services;

namespace Xenon.Web.Pages.Admin
{
    public class ImportModel : PageModel
    {
        private readonly ProductImportService _importService;
        private readonly ILogger<ImportModel> _logger;

        public ImportModel(ProductImportService importService, ILogger<ImportModel> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile? CsvFile { get; set; }

        public string? ResultMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (CsvFile == null || CsvFile.Length == 0)
            {
                ResultMessage = "Файл не выбран";
                return Page();
            }

            try
            {
                using var stream = CsvFile.OpenReadStream();
                var count = await _importService.ImportFromCsvAsync(stream);
                ResultMessage = $"Импортировано {count} товаров";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import failed");
                ResultMessage = $"Ошибка: {ex.Message}";
            }

            return Page();
        }
    }
}