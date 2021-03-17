using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugins.Telegram.Configuration;
using System.Text.Json;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Model.Serialization;
using System.Net.Mime;
using Jellyfin.Data.Entities;
using System.Net.Http;

namespace Jellyfin.Plugins.Telegram
{
    public class Notifier : INotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Notifier> _logger;

        public Notifier(ILogger<Notifier> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public static async Task SendNotification(IHttpClientFactory httpClientFactory, string token, string chatId, bool fSilentNotificationEnabled, string text)
        {
            var body = new Dictionary<string, object>() {
                {"chat_id", chatId},
                {"text", text},
                {"parse_mode","html"},
                {"disable_notification", fSilentNotificationEnabled},
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.telegram.org/bot{token}/sendMessage");
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(body),
            System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);

            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            using var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        }

        public async Task SendNotification(UserNotification request, CancellationToken cancellationToken)
        {
            var options = GetOptions(request.User);

            await Notifier.SendNotification(
                _httpClientFactory,
                options.Token, options.ChatId, options.SilentNotificationEnabled,
                string.IsNullOrEmpty(request.Description) ? request.Name : request.Description
            );
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);
            return options != null && IsValid(options) && options.Enabled;
        }

        public string Name => Plugin.Instance!.Name;

        private static TelegramOptions GetOptions(User user)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(u => string.Equals(u.UserId, user.Id.ToString("N"),
                    StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsValid(TelegramOptions options)
        {
            return !string.IsNullOrEmpty(options.Token)
                   && !string.IsNullOrEmpty(options.ChatId);
        }
    }
}