using MakeenBot.Interfaces.Handlers;
using MakeenBot.Interfaces.Services;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot;
using MakeenBot.Models;

namespace MakeenBot.Handlers.Report
{
    public class AddReportHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly IReportService _reportService;
        private readonly BotConfig _config;
        private readonly ILogger<AddReportHandler> _logger;

        public AddReportHandler(
            IOptions<BotConfig> config,
            IReportService reportService,
            ILogger<AddReportHandler> logger)
        {
            _config = config.Value;
            _reportService = reportService;
            _logger = logger;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
        }

        public bool CanHandle(string messageText)
        {
            return messageText.Contains("#گزارش_روزانه", StringComparison.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(Message message)
        {
            try
            {
                var isEdit = message.EditDate.HasValue;

                var (isSuccess, responseMessage) = await _reportService.HandleReportSubmissionAsync(
                    message.Text,
                    isEdit: isEdit
                );

                await _bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: responseMessage,
                    replyParameters: message.MessageId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطا در ثبت گزارش روزانه");

                await _bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: "❌ خطایی در ثبت گزارش رخ داد.",
                    replyParameters: message.MessageId
                );
            }
        }
    }
}
