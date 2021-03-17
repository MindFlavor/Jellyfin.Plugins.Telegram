using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Jellyfin.Plugins.Telegram.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Jellyfin.Plugins.Telegram.Api
{
    [ApiController]
    [Route("Notification/Telegram")]
    [Produces(MediaTypeNames.Application.Json)]
    public class TelegramNotificationsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        public TelegramNotificationsController(IHttpClientFactory httpClientFactory, IJsonSerializer jsonSerializer, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializer = jsonSerializer;
            _logger = loggerFactory.CreateLogger<TelegramNotificationsController>();
        }

        private static TelegramOptions GetOptions(string userId)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(u => string.Equals(u.UserId, userId,
                    StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost("Test/{userId}")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostAsync([FromRoute] string userId)
        {
            var options = GetOptions(userId);

            await Notifier.SendNotification(
                _httpClientFactory,
                options.Token, options.ChatId, options.SilentNotificationEnabled,
                "This is a test notification from Jellyfin"
            );

            return NoContent();
        }
    }
}
