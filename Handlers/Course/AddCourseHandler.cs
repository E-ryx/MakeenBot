using MakeenBot.Interfaces.Handlers;
using MakeenBot.Models.Entities;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot;
using MakeenBot.Models;

namespace MakeenBot.Handlers.Course
{
    public class AddCourseHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotConfig _config;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<AddCourseHandler> _logger;

        public AddCourseHandler(
            IOptions<BotConfig> config,
            ICourseRepository courseRepository,
            ILogger<AddCourseHandler> logger)
        {
            _config = config.Value;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public bool CanHandle(string messageText)
        {
            return messageText.StartsWith("/addcourse", StringComparison.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(Message message)
        {
            if (message.From?.Id != _config.AuthorizedUserId)
            {
                await _bot.SendMessage(message.Chat.Id, "⛔ شما اجازه‌ی ثبت دوره را ندارید.");
                return;
            }

            var lines = message.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var courseLine = lines.FirstOrDefault(l => l.StartsWith("اسم دوره:", StringComparison.OrdinalIgnoreCase));
            var courseName = courseLine?.Split(':')[1].Trim();

            var studentSectionIndex = Array.FindIndex(lines, l => l.StartsWith("دانشجو های دوره:", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(courseName) || studentSectionIndex == -1 || studentSectionIndex + 1 >= lines.Length)
            {
                await _bot.SendMessage(message.Chat.Id, "❌ فرمت پیام اشتباه است. لطفاً از ساختار زیر استفاده کنید:\n\n" +
                    "/add-course\nاسم دوره: مثال\nدانشجو های دوره:\n- علی\n- زهرا");
                return;
            }

            // بررسی تکراری بودن دوره
            if (await _courseRepository.ExistsAsync(courseName))
            {
                await _bot.SendMessage(message.Chat.Id, $"⚠️ دوره‌ای با نام «{courseName}» از قبل وجود دارد.");
                return;
            }

            var studentLines = lines
                .Skip(studentSectionIndex + 1)
                .Select(l => l.TrimStart('-', ' ', '\t').Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            if (!studentLines.Any())
            {
                await _bot.SendMessage(message.Chat.Id, "❌ هیچ دانشجویی وارد نشده است.");
                return;
            }

            // ثبت دوره جدید
            var course = new Models.Entities.Course(courseName);
            foreach (var studentName in studentLines)
            {
                course.Students.Add(new Student(studentName, course.Id));
            }

            await _courseRepository.AddAsync(course);

            await _bot.SendMessage(
                message.Chat.Id,
                $"✅ دوره «{courseName}» با {course.Students.Count} دانشجو با موفقیت ذخیره شد. 🎉"
            );
        }
    }
}
