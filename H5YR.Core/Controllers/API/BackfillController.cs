using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Controllers.API
{
    [Route("api/admin/backfill")]
    [ApiController]
    public class BackfillController : ControllerBase
    {
        private readonly ILogger<BackfillController> _logger;
        private readonly FeedItemLogBackfillService _backfillService;

        public BackfillController(
            ILogger<BackfillController> logger,
            FeedItemLogBackfillService backfillService)
        {
            _logger = logger;
            _backfillService = backfillService;
        }

        /// <summary>
        /// Manually trigger FeedItemLog backfill
        /// DELETE /api/admin/backfill to run the backfill
        /// </summary>
        [HttpPost("run")]
        public async Task<IActionResult> RunBackfill()
        {
            _logger.LogInformation("Manual backfill triggered via API");

            try
            {
                _backfillService.BackfillFromWidget();
                await _backfillService.BackfillFromMastodonAsync();
                return Ok(new { message = "Backfill completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manual backfill failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
