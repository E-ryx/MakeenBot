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
[Route("api")]
public class GroupController : ControllerBase
{
    private readonly BotConfig _setting;
    private readonly string BotToken;
    private readonly string CustomApiUrl;
    private readonly string webhookUrl;

    private readonly IReportService _reportService;
    private readonly IReportRepository _reportRepository;

    private readonly ITelegramBotClient bot;

    public GroupController(IReportService reportService, IReportRepository reportRepository, IOptions<BotConfig> options)
    {
        _setting = options.Value;
        BotToken = _setting.Token;
        CustomApiUrl = _setting.BaleApi;
        webhookUrl = _setting.Webhook;
        _reportService = reportService;
        _reportRepository = reportRepository;
        bot = new TelegramBotClient(new TelegramBotClientOptions(BotToken, CustomApiUrl));
    }
    
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook(CancellationToken ct)
    {
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
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

                var report = _reportService.ParseDailyReport(text);

                if (report != null)
                {
                    Console.WriteLine("Valid Report!");
                    Console.WriteLine($"Name: {report.NameTag}");
                    Console.WriteLine($"Date: {report.PersianDate}");
                    Console.WriteLine($"WorkHour: {report.WorkHour}");
                    Console.WriteLine($"ReportNumber: {report.ReportNumber}");

                    // Save the report to the repository
                    var result = await _reportRepository.SaveReportAsync(report);
                    if (!result.Success)
                    {
                        //Report already exist.
                         await bot.SendMessage(msg.Chat.Id, $"{result.Message}");
                    }

                    // Succes Submit.
                     await bot.SendMessage(msg.Chat.Id, $"{result.Message}");

                    // Optionally respond to group:
                    await bot.SendMessage(msg.Chat.Id, $"📬 گزارش شما با موفقیت دریافت شد.");
                }
                else
                {
                    Console.WriteLine("Invalid report format.");
                    // Optionally respond to guide the user:
                     await bot.SendMessage(msg.Chat.Id, $"❌ فرمت گزارش معتبر نیست. لطفاً مطابق نمونه ارسال کنید.");
                }
            }
            else
            {
                Console.WriteLine($"[{group}] {user} ({firstName}): {text}");
            }
        }

        return Ok();
    }
}
