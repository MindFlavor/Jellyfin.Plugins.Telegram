using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugins.Telegram.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugins.Telegram
{
    public class Notifier : INotificationService
    {
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;

        public Notifier(IHttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        public static async Task SendNotification(IHttpClient httpClient, IJsonSerializer jsonSerializer, string token, string chatId, string text)
        {
            var body = new Dictionary<string, object>() {
                {"chat_id", chatId},
                {"text", text},
                {"parse_mode","html"}
            };

            var requestOptions = new HttpRequestOptions
            {
                Url = $"https://api.telegram.org/bot{token}/sendMessage",
                RequestContent = jsonSerializer.SerializeToString(body),
                RequestContentType = "application/json",
                LogErrorResponseBody = true
            };

            await httpClient.Post(requestOptions).ConfigureAwait(false);
        }

        public async Task SendNotification(UserNotification request, CancellationToken cancellationToken)
        {
            var options = GetOptions(request.User);

            await Notifier.SendNotification(
                _httpClient, _jsonSerializer,
                options.Token, options.ChatId,
                string.IsNullOrEmpty(request.Description) ? request.Name : request.Description
            );
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);
            return options != null && IsValid(options) && options.Enabled;
        }

        public string Name => Plugin.Instance.Name;

        private static TelegramOptions GetOptions(BaseItem user)
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