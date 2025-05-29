using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;
using MakeenBot.Models.ValueObjects;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Interfaces.Services;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly BotConfig _settings;
    private readonly ITelegramBotClient _bot;
    private readonly IReportService _reportService;
    private readonly IReportRepository _reportRepository;

    public BotController(IReportService reportService, IReportRepository reportRepository, IOptions<BotConfig> options)
    {
        _settings = options.Value;
        _bot = new TelegramBotClient(new TelegramBotClientOptions(_settings.Token, _settings.BaleApi));
        _reportService = reportService;
        _reportRepository = reportRepository;
    }

    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook(CancellationToken ct)
    {
        await _bot.SetWebhook(_settings.Webhook, allowedUpdates: null, cancellationToken: ct);
        return $"Webhook set to {_settings.Webhook}";
    }

    [HttpPost("reports")]
    public async Task<IActionResult> GetReports([FromBody] Update update)
    {
        if (IsValidGroupMessage(update))
        {
            var msg = update.Message;
            var text = msg.Text ?? string.Empty;

            if (text.Contains("#گزارش_روزانه", StringComparison.OrdinalIgnoreCase))
            {
                return await HandleDailyReport(msg, text);
            }
        }

        return Ok();
    }

    private bool IsValidGroupMessage(Update update)
    {
        return update.Type == UpdateType.Message && update.Message?.Chat?.Type == ChatType.Group;
    }

    private async Task<IActionResult> HandleDailyReport(Message msg, string text)
    {
        var report = _reportService.ParseDailyReport(text);

        if (report == null)
            await _bot.SendMessage(msg.Chat.Id, "❌ فرمت گزارش معتبر نیست. لطفاً مطابق نمونه ارسال کنید.");

        var result = await _reportRepository.SaveReportAsync(report);
        await _bot.SendMessage(msg.Chat.Id, $"{result.Message}");

        return Ok();
    }
}
