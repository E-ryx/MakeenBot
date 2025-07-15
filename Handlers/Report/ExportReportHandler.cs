using MakeenBot.Interfaces.Handlers;
using MakeenBot.Interfaces.Services;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot;
using MakeenBot.Models;

namespace MakeenBot.Handlers.Report
{
        public class ExportReportHandler : IBotCommandHandler
        {
            private readonly ITelegramBotClient _bot;
            private readonly IReportService _reportService;
            private readonly BotConfig _config;
            private readonly ILogger<ExportReportHandler> _logger;

        public ExportReportHandler(
            IOptions<BotConfig> config,
            ILogger<ExportReportHandler> logger,
            IReportService reportService)
        {
            _config = config.Value;
            _logger = logger;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
            _reportService = reportService;
        }

        public bool CanHandle(string messageText)
            {
                return messageText.StartsWith("/report");
            }

            public async Task HandleAsync(Message message)
            {
                if (message.From?.Id != _config.AuthorizedUserId)
                {
                    await _bot.SendMessage(message.Chat.Id, "⛔ شما اجازه‌ی دریافت گزارش را ندارید.");
                    return;
                }

                var lines = message.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var courseName = lines.FirstOrDefault(l => l.StartsWith("دوره:"))?.Split(':')[1].Trim();
                var fromDateStr = lines.FirstOrDefault(l => l.StartsWith("از:"))?.Split(':')[1].Trim();
                var toDateStr = lines.FirstOrDefault(l => l.StartsWith("تا:"))?.Split(':')[1].Trim();

                if (courseName == null || fromDateStr == null || toDateStr == null)
                {
                    await _bot.SendMessage(message.Chat.Id,
                        "❌ لطفاً اطلاعات را با فرمت صحیح وارد کنید:\n\n" +
                        "/گزارش خروجی\nدوره: نام دوره\nاز: 1403/01/01\nتا: 1403/01/10");
                    return;
                }

                try
                {
                    var startDate = ParsePersianDate(fromDateStr);
                    var endDate = ParsePersianDate(toDateStr);

                    var stream = await _reportService.ExportReportsToExcelAsync(courseName, startDate, endDate);

                    if (stream == null)
                    {
                        await _bot.SendMessage(message.Chat.Id, $"❌ گزارشی برای دوره «{courseName}» یافت نشد.");
                        return;
                    }

                if (stream.Length > 20 * 1024 * 1024) // 20MB
                {
                    await _bot.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ حجم فایل گزارش بیش از حد مجاز تلگرام است."
                    );
                    return;
                }

                stream.Position = 0;

                await _bot.SendDocument(
                   chatId: message.Chat.Id,
                   document: new InputFileStream(stream, $"{courseName}.xlsx"),
                   caption: $"📤 گزارش از {fromDateStr} تا {toDateStr}"
                );
            }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در تولید گزارش");
                    await _bot.SendMessage(message.Chat.Id, "❌ خطایی در پردازش گزارش رخ داد.");
                }
            }

            private DateTime ParsePersianDate(string persianDate)
            {
                var parts = persianDate.Split('/');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);
                var pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
        }
}
