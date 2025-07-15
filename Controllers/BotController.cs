using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using MakeenBot.Interfaces;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MakeenBot.Interfaces.Handlers;
using MakeenBot.Interfaces.Services;
using MakeenBot.Models;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly ITelegramBotClient _bot;
    private readonly IEnumerable<IBotCommandHandler> _commandHandlers;
    private readonly BotConfig _settings;
    private readonly ILogger<BotController> _logger;

    public BotController(
        IOptions<BotConfig> options,
        ILogger<BotController> logger,
        IEnumerable<IBotCommandHandler> commandHandlers)
    {
        _settings = options.Value;
        _bot = new TelegramBotClient(new TelegramBotClientOptions(_settings.Token, _settings.BaleApi));
        _logger = logger;
        _commandHandlers = commandHandlers;
    }

    [HttpGet("setWebhook")]
    public async Task<string> SetWebhook(CancellationToken ct)
    {
        await _bot.SetWebhook(_settings.Webhook, cancellationToken: ct);
        return $"✅ Webhook set to {_settings.Webhook}";
    }

    [HttpPost("update")]
    public async Task<IActionResult> ReceiveUpdate([FromBody] Update update)
    {
        try
        {
            // 1️⃣ پیام خوش‌آمدگویی هنگام اضافه شدن ربات
            if (update.Message?.NewChatMembers != null &&
                update.Message.NewChatMembers.Any(m => m.IsBot && m.Id == _settings.BotId))
            {
                var welcomePath = Path.Combine("Messages", "WelcomeMessage.md");
                var welcomeText = await System.IO.File.ReadAllTextAsync(welcomePath);

                await _bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: welcomeText,
                    parseMode: ParseMode.Markdown
                );

                return Ok();
            }

            // 2️⃣ بررسی نوع آپدیت و متن پیام
            var message = update.Type switch
            {
                UpdateType.Message => update.Message,
                UpdateType.EditedMessage => update.EditedMessage,
                _ => null
            };

            if (message == null || string.IsNullOrWhiteSpace(message.Text))
                return Ok();

            // 3️⃣ سپردن به CommandHandler مناسب
            foreach (var handler in _commandHandlers)
            {
                if (handler.CanHandle(message.Text))
                {
                    await handler.HandleAsync(message);
                    return Ok();
                }
            }

            return Ok(); // پیام‌هایی که نیاز به پاسخ ندارند
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ خطا در پردازش آپدیت");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
