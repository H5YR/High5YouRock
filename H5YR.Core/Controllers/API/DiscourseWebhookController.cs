using H5YR.Core.Data.Constants;
using H5YR.Core.Data.Entities;
using H5YR.Core.Models.Discourse;
using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Scoping;

namespace H5YR.Core.Controllers.API
{
    [ApiController]
    [Route("api/discourse")]
    public class DiscourseWebhookController : ControllerBase
    {
        private readonly ILogger<DiscourseWebhookController> _logger;
        private readonly IDiscourseService _discourseService;
        private readonly IScopeProvider _scopeProvider;

        public DiscourseWebhookController(
            ILogger<DiscourseWebhookController> logger,
            IDiscourseService discourseService,
            IScopeProvider scopeProvider)
        {
            _logger = logger;
            _discourseService = discourseService;
            _scopeProvider = scopeProvider;
        }

        /// <summary>
        /// Webhook endpoint for Discourse to send like notifications
        /// POST: /api/discourse/webhook
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] DiscourseWebhookPayload payload)
        {
            _logger.LogInformation("Received Discourse webhook");

            if (payload == null)
            {
                _logger.LogWarning("Received null payload");
                return BadRequest(new { error = "Invalid payload" });
            }

            var success = await _discourseService.ProcessWebhookAsync(payload);

            if (success)
            {
                return Ok(new { message = "Webhook processed successfully" });
            }

            return StatusCode(500, new { error = "Failed to process webhook" });
        }

        /// <summary>
        /// Get recent Discourse reactions
        /// GET: /api/discourse/recent?count=10
        /// </summary>
        [HttpGet("recent")]
        public IActionResult GetRecent([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100)
            {
                return BadRequest(new { error = "Count must be between 1 and 100" });
            }

            var reactions = _discourseService.GetRecentReactions(count);
            return Ok(reactions);
        }

        /// <summary>
        /// Get total count of Discourse reactions
        /// GET: /api/discourse/count
        /// </summary>
        [HttpGet("count")]
        public IActionResult GetCount()
        {
            var count = _discourseService.GetTotalReactionCount();
            return Ok(new { count });
        }

        /// <summary>
        /// Health check endpoint
        /// GET: /api/discourse/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new 
            { 
                status = "healthy",
                service = "Discourse Webhook API",
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Debug endpoint to check if table exists
        /// GET: /api/discourse/debug/table
        /// </summary>
        [HttpGet("debug/table")]
        public IActionResult CheckTable()
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();
                var sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{DiscourseReactionSchemaConstants.TableName}'";
                var result = scope.Database.ExecuteScalar<string>(sql);
                bool tableExists = !string.IsNullOrEmpty(result);

                scope.Complete();

                return Ok(new
                {
                    tableName = DiscourseReactionSchemaConstants.TableName,
                    exists = tableExists,
                    message = tableExists 
                        ? "Table exists" 
                        : "Table does NOT exist. Please restart the application to trigger the migration, or check the logs for migration errors."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}