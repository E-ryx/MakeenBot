using MakeenBot.Interfaces.Handlers;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using MakeenBot.Handlers.WelcomeMessage;
using MakeenBot.Models;

namespace MakeenBot.Handlers.HelpMessage
{
    public class HelpMessageHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotConfig _config;
        private readonly ILogger<HelpMessageHandler> _logger;

        public HelpMessageHandler(IOptions<BotConfig> config, ILogger<HelpMessageHandler> logger)
        {
            _config = config.Value;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
            _logger = logger;
        }

        public bool CanHandle(string messageText)
        {
            return messageText.Trim().StartsWith("/help", StringComparison.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(Telegram.Bot.Types.Message message)
        {
            if (message.From?.Id != _config.AuthorizedUserId)
            {
                return;
            }

            var startPath = Path.Combine("Messages", "HelpMessage.txt");
            var startText = await System.IO.File.ReadAllTextAsync(startPath);

            await _bot.SendMessage(
                chatId: message.Chat.Id,
                text: startText,
                parseMode: ParseMode.Markdown
            );
        }
    }
}
