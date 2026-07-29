using Microsoft.AspNetCore.Mvc;
using Xenon.Infrastructure.Services;

namespace Xenon.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly ProductImportService _importService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(ProductImportService importService, ILogger<ImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpPost("csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest("Only CSV files are allowed");

            try
            {
                using var stream = file.OpenReadStream();
                var count = await _importService.ImportFromCsvAsync(stream);
                return Ok(new { imported = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import failed");
                return StatusCode(500, ex.Message);
            }
        }
    }
}