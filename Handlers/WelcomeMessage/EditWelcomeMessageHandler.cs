    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using MakeenBot.Models.ValueObjects;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using MakeenBot.Interfaces.Handlers;

namespace MakeenBot.Handlers.WelcomeMessage
{

    public class EditWelcomeMessageHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotConfig _config;
        private readonly ILogger<EditWelcomeMessageHandler> _logger;

        public EditWelcomeMessageHandler(IOptions<BotConfig> config, ILogger<EditWelcomeMessageHandler> logger)
        {
            _config = config.Value;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
            _logger = logger;
        }

        public bool CanHandle(string messageText)
        {
            return messageText.StartsWith("/welcomemessage", StringComparison.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(Message message)
        {
            if (message.From?.Id != _config.AuthorizedUserId)
            {
                await _bot.SendMessage(message.Chat.Id, "⛔ شما مجاز به تغییر پیام خوش‌آمدگویی نیستید.");
                return;
            }

            var lines = message.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (lines.Count < 2)
            {
                await _bot.SendMessage(
                    message.Chat.Id,
                    "❌ لطفاً پیام جدید را بعد از دستور وارد کنید:\n\n/welcome-message\nمتن پیام"
                );
                return;
            }

            var newText = string.Join('\n', lines.Skip(1)).Trim();
            var path = Path.Combine("Messages", "WelcomeMessage.md");

            try
            {
                Directory.CreateDirectory("Messages");
                await File.WriteAllTextAsync(path, newText);

                await _bot.SendMessage(
                    message.Chat.Id,
                    "✅ پیام خوش‌آمدگویی با موفقیت ذخیره شد."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطا در ذخیره پیام خوش‌آمدگویی");
                await _bot.SendMessage(
                    message.Chat.Id,
                    "❌ خطا در ذخیره پیام. لطفاً بعداً تلاش کنید."
                );
            }
        }
    }

}
