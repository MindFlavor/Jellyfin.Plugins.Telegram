namespace Jellyfin.Plugins.Telegram.Configuration
{
    public class TelegramOptions
    {
        public string UserId { get; set; }
        public bool Enabled { get; set; }
        public string Token { get; set; }
        public string ChatId { get; set; }

    }
}