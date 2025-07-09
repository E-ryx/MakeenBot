using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Threading;
using System.Threading.Tasks;
using MakeenBot.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MakeenBot.Interfaces.Services;
using MakeenBot.Models.ValueObjects;
using MakeenBot.Services;
using System.Globalization;
using MakeenBot.Models.Entities;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly ITelegramBotClient _bot;
    private readonly IReportService _reportService;
    private readonly IExportService _exportService;
    private readonly ICourseRepository _courseRepository;
    private readonly BotConfig _settings;
    private readonly ILogger<BotController> _logger;

    public BotController(
        IReportService reportService,
        IOptions<BotConfig> options,
        ILogger<BotController> logger,
        IExportService exportService,
        ICourseRepository courseRepository)
    {
        _settings = options.Value;
        _bot = new TelegramBotClient(new TelegramBotClientOptions(_settings.Token, _settings.BaleApi));
        _reportService = reportService;
        _exportService = exportService;
        _logger = logger;
        _courseRepository = courseRepository;
    }

    [HttpGet("setWebhook")]
    public async Task<string> SetWebhook(CancellationToken ct)
    {
        await _bot.SetWebhook(_settings.Webhook, allowedUpdates: null, cancellationToken: ct);
        return $"✅ Webhook set to {_settings.Webhook}";
    }

    [HttpPost("update")]
    public async Task<IActionResult> ReceiveUpdate([FromBody] Update update)
    {
        var message = update.Type switch
        {
            UpdateType.Message => update.Message,
            UpdateType.EditedMessage => update.EditedMessage,
            _ => null
        };

        if (message?.Chat?.Type == ChatType.Channel)
            return Ok(); 

        var text = message.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return Ok();

        // --- بررسی دسترسی ---
        if (message.Chat.Type != ChatType.Private || message.From?.Id != _settings.AuthorizedUserId)
            return Ok(); // دسترسی غیرمجاز

        try
        {
            if (text.StartsWith("/گزارش خروجی"))
            {
                var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var courseName = lines.FirstOrDefault(l => l.StartsWith("دوره:"))?.Split(':')[1].Trim();
                var fromDateStr = lines.FirstOrDefault(l => l.StartsWith("از:"))?.Split(':')[1].Trim();
                var toDateStr = lines.FirstOrDefault(l => l.StartsWith("تا:"))?.Split(':')[1].Trim();

                if (courseName == null || fromDateStr == null || toDateStr == null)
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ فرمت پیام نادرست است. لطفاً دوره، از، و تا را مشخص کنید."
                        //, replyToMessageId: message.MessageId
                        );
                    return Ok();
                }

                var startDate = ParsePersianDate(fromDateStr);
                var endDate = ParsePersianDate(toDateStr);

                //var exportService = HttpContext.RequestServices.GetRequiredService<ExportService>();
                var stream = await _exportService.ExportReportsToExcelAsync(courseName, startDate, endDate);
                if (stream == null)
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ دوره‌ای با این نام یافت نشد."
                        //, replyToMessageId: message.MessageId
                        );
                    return Ok();
                }

                stream.Position = 0;
                //await _bot.SendDocument(
                //    chatId: message.Chat.Id,
                //    document: new Telegram.Bot.Types.InputFileStream(stream, $"{courseName}.xlsx"),
                //    caption: $"از تاریخ {fromDateStr} تا {toDateStr}"
                //    //,replyToMessageId: message.MessageId
                //);

                //return Ok();
                return File(
                    fileStream: stream,
        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        fileDownloadName: $"{courseName}.xlsx"
                    );
            }

            if (text.StartsWith("/add-course", StringComparison.OrdinalIgnoreCase))
            {
                var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var courseLine = lines.FirstOrDefault(l => l.StartsWith("اسم دوره:", StringComparison.OrdinalIgnoreCase));
                var courseName = courseLine?.Split(':')[1].Trim();


                var studentSectionIndex = Array.FindIndex(lines, l => l.StartsWith("دانشجو های دوره:", StringComparison.OrdinalIgnoreCase));
                if (courseName == null || studentSectionIndex == -1 || studentSectionIndex + 1 >= lines.Length)
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ فرمت پیام نامعتبر است. لطفاً از ساختار صحیح استفاده کنید.");
                    return Ok();
                }

                if (await _courseRepository.ExistsAsync(courseName))
                {
                    await _bot.SendMessage(message.Chat.Id, $"⚠️ دوره‌ای با نام «{courseName}» از قبل وجود دارد.");
                    return Ok(new { success = false, message = "Course already exists" });
                }

                var studentLines = lines
                    .Skip(studentSectionIndex + 1)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.TrimStart('-', ' ', '\t').Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                if (!studentLines.Any())
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ هیچ دانشجویی در پیام وارد نشده است.");
                    return Ok();
                }

                var course = new Course(courseName);
                foreach (var student in studentLines)
                {
                    course.Students.Add(new Student(student, course.Id));
                }

                await _courseRepository.AddAsync(course);

                await _bot.SendMessage(message.Chat.Id, $"✅ دوره «{courseName}» با {course.Students.Count} دانشجو با موفقیت ثبت شد.");
                return Ok();
            }

            // Update Course
            if (text.StartsWith("/edit-course", StringComparison.OrdinalIgnoreCase))
            {
                var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var courseLine = lines.FirstOrDefault(l => l.StartsWith("اسم دوره:", StringComparison.OrdinalIgnoreCase));
                var courseName = courseLine?.Split(':')[1].Trim();

                var studentSectionIndex = Array.FindIndex(lines, l => l.StartsWith("دانشجو های دوره:", StringComparison.OrdinalIgnoreCase));
                if (courseName == null || studentSectionIndex == -1 || studentSectionIndex + 1 >= lines.Length)
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ فرمت پیام نامعتبر است. لطفاً از ساختار صحیح استفاده کنید.");
                    return Ok();
                }

                var existingCourse = await _courseRepository.GetByNameAsync(courseName);
                if (existingCourse == null)
                {
                    await _bot.SendMessage(message.Chat.Id, $"❌ دوره‌ای با نام «{courseName}» یافت نشد.");
                    return Ok(new { success = false, message = "Course not found" });
                }

                var studentLines = lines
                    .Skip(studentSectionIndex + 1)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.TrimStart('-', ' ', '\t').Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                if (!studentLines.Any())
                {
                    await _bot.SendMessage(message.Chat.Id, "❌ هیچ دانشجویی در پیام وارد نشده است.");
                    return Ok();
                }
                var newStudents = studentLines.Select(name => new Student(name, existingCourse.Id)).ToList();
                await _courseRepository.UpdateStudentsAsync(existingCourse, newStudents);

                await _bot.SendMessage(message.Chat.Id, $"✅ دوره «{courseName}» با {existingCourse.Students.Count} دانشجو بروزرسانی شد.");
                return Ok();
            }

            // --- گزارش روزانه ---
            if (!text.Contains("#گزارش_روزانه", StringComparison.OrdinalIgnoreCase))
                return Ok();

            var isEdit = update.Type == UpdateType.EditedMessage;

            var (isSuccess, responseMessage) = await _reportService.HandleReportSubmissionAsync(
                text,
                isEdit: isEdit
            );

            //await _bot.SendMessage(
            //    chatId: message.Chat.Id,
            //    text: responseMessage,
            //    replyParameters: new() { MessageId = message.MessageId },
            //    cancellationToken: CancellationToken.None
            //);

            return Ok(new { success = isSuccess, message = responseMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ خطا در پردازش پیام");

            await _bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ مشکلی در پردازش پیام پیش آمد.",
                cancellationToken: CancellationToken.None
            );

            return StatusCode(500, "Internal Server Error");
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