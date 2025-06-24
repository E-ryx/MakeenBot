using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Threading;
using System.Threading.Tasks;
using MakeenBot.Interfaces;
using MakeenBot.Repositories;
using Microsoft.Extensions.Options;
using MakeenBot.Models.Entities;
using MakeenBot.Models.ValueObjects;
using MakeenBot.Interfaces.Validators;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly BotConfig _settings;
    private readonly ITelegramBotClient _bot;
    private readonly IReportService _reportService;
    private readonly IReportRepository _reportRepository;
    private readonly IReportValidator _reportValidator;

    public BotController(IReportService reportService, IReportRepository reportRepository, IOptions<BotConfig> options, IReportValidator reportValidator)
    {
        _settings = options.Value;
        _bot = new TelegramBotClient(new TelegramBotClientOptions(_settings.Token, _settings.BaleApi));
        _reportService = reportService;
        _reportRepository = reportRepository;
        _reportValidator = reportValidator;
    }

    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook(CancellationToken ct)
    {
        await _bot.SetWebhook(_settings.Webhook, allowedUpdates: null, cancellationToken: ct);
        return $"Webhook set to {_settings.Webhook}";
    }

    [HttpPost("update")]
    public async Task<IActionResult> GetUpdate([FromBody] Update update)
    {
        var response = _reportValidator.ValidateReport(update);
        if (!response.IsValid)
        {
            // var msg = update.Message;
            // var text = msg.Text ?? string.Empty;

            // if (text.Contains("#گزارش_روزانه", StringComparison.OrdinalIgnoreCase))
            // {
            //     return await HandleDailyReport(msg, text);
            // }
            
            await _bot.SendMessage(response.ErrorMessage);
        }

        if (update.Type == UpdateType.Message && update.Message?.Chat?.Type == ChatType.Group)
        {
            // Go to ReportService.AddReport
        }

        return Ok();
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
