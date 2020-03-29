# Jellyfin.Plugins.Telegram
Telegram bot Notification Agent for Jellyfin

Get started with Gotify at https://gotify.net/

Get started with Jellyfin at https://jellyfin.github.io/

## How to use

* Create a bot (or use an existing one). You can use the `/newbot` command of BotFather for this. Make sure to copy the access token.
* Start the bot (by sending the `/start` command).
* Say something to your bot (a simple hello will suffice).
* Get your Chat Id. You can GET this URL: `https://api.telegram.org/bot<<YOUR TOAKEN>>/getUpdates`. Of course you will need to replace `<<YOUR TOKEN>>` with your token. In the answer, copy the `chat_id`.
* Now enable the plugin in Jellyfin specifiyng both the access token and the chat id. Enable, save the configuration and test it. If everything is ok you will receive the test notification in your bot.
* Profit!
