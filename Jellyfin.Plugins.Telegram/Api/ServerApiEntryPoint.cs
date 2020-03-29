using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Plugins.Telegram.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugins.Telegram.Api
{
    [Route("/Notification/Telegram/Test/{UserId}", "Post", Summary = "Tests Telegram")]
    public class TestNotification : IReturnVoid
    {
        [ApiMember(Name = "UserId", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserId { get; set; }
    }

    public class ServerApiEndpoints : IService
    {
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        public ServerApiEndpoints(IHttpClient httpClient, IJsonSerializer jsonSerializer, ILoggerFactory loggerFactory)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _logger = loggerFactory.CreateLogger<ServerApiEndpoints>();
        }

        private static TelegramOptions GetOptions(string userId)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(u => string.Equals(u.UserId, userId,
                    StringComparison.OrdinalIgnoreCase));
        }

        private async Task PostAsync(TestNotification request)
        {
            var options = GetOptions(request.UserId);

            await Notifier.SendNotification(
                _httpClient, _jsonSerializer,
                options.Token, options.ChatId,
                "This is a test notification from Jellyfin"
            );
        }

        public void Post(TestNotification request)
        {
            PostAsync(request)
                .GetAwaiter()
                .GetResult();
        }
    }
}
