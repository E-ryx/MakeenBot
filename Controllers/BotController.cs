using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Text.RegularExpressions;
using MakeenBot.Interfaces;
using MakeenBot.Repositories;
using MakeenBot.Models;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    // Config's
    private readonly BotConfig _setting;
    private readonly string BotToken;
    private readonly string CustomApiUrl;
    private readonly string webhookUrl;
    private readonly ITelegramBotClient bot;

    // Service's
    private readonly IReportService _reportService;
    private readonly IReportRepository _reportRepository;

    public BotController(IReportService reportService, IReportRepository reportRepository, IOptions<BotConfig> options)
    {
        // Config's.
        _setting = options.Value;
        BotToken = _setting.Token;
        CustomApiUrl = _setting.BaleApi;
        webhookUrl = _setting.Webhook;
        bot = new TelegramBotClient(new TelegramBotClientOptions(BotToken, CustomApiUrl));
        
        // Service's.
        _reportService = reportService;
        _reportRepository = reportRepository;
    }
    
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook(CancellationToken ct)
    {
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost("reports")]
    public async Task<IActionResult> Reports([FromBody] Update update)
    {
        if (update.Type == UpdateType.Message && update.Message?.Chat?.Type == ChatType.Group)
        {
            var msg = update.Message;
            var text = msg.Text ?? "";
            var user = msg.From?.Username ?? "Unknown";
            var firstName = msg.From?.FirstName ?? "Unknown";
            var group = msg.Chat.Title ?? "Unknown Group";

            if (text.Contains("#گزارش_روزانه", StringComparison.OrdinalIgnoreCase))
            {
                
                Console.WriteLine("Report Detected. Validating...");

                // Parse Report.
                var report = _reportService.ParseDailyReport(text);

                if (report != null)
                {
                    // Console Log's
                    Console.WriteLine("Valid Report!");
                    Console.WriteLine($"Name: {report.NameTag}");
                    Console.WriteLine($"Date: {report.PersianDate}");
                    Console.WriteLine($"WorkHour: {report.WorkHour}");
                    Console.WriteLine($"ReportNumber: {report.ReportNumber}");

                    // Save Reports.
                    var result = await _reportRepository.SaveReportAsync(report);

                    // Response Message.
                     await bot.SendMessage(msg.Chat.Id, $"{result.Message}");
                }
                else
                {
                    Console.WriteLine("Invalid report format.");
                    // Response Message: Not Valid Report Format.
                     await bot.SendMessage(msg.Chat.Id, $"❌ فرمت گزارش معتبر نیست. لطفاً مطابق نمونه ارسال کنید.");
                }
            }
            else
            {
                // Console Log's
                Console.WriteLine($"[{group}] {user} ({firstName}): {text}");
            }
        }

        return Ok();
    }
}
