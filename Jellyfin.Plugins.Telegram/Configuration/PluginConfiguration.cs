using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugins.Telegram.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public TelegramOptions[] Options { get; set; }

        public PluginConfiguration()
        {
            Options = new TelegramOptions[0];
        }
    }

}
