using MakeenBot.Interfaces.Handlers;
using MakeenBot.Models.Entities;
using MakeenBot.Models.ValueObjects;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace MakeenBot.Handlers.Course
{
    public class EditCourseHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly BotConfig _config;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<EditCourseHandler> _logger;

        public EditCourseHandler(
            IOptions<BotConfig> config,
            ICourseRepository courseRepository,
            ILogger<EditCourseHandler> logger)
        {
            _config = config.Value;
            _bot = new TelegramBotClient(new TelegramBotClientOptions(_config.Token, _config.BaleApi));
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public bool CanHandle(string messageText)
        {
            return messageText.StartsWith("/editcourse", StringComparison.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(Message message)
        {
            if (message.From?.Id != _config.AuthorizedUserId)
            {
                await _bot.SendMessage(message.Chat.Id, "⛔ شما اجازه‌ی ویرایش دوره را ندارید.");
                return;
            }

            var lines = message.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var courseLine = lines.FirstOrDefault(l => l.StartsWith("اسم دوره:", StringComparison.OrdinalIgnoreCase));
            var courseName = courseLine?.Split(':')[1].Trim();

            var studentSectionIndex = Array.FindIndex(lines, l => l.StartsWith("دانشجو های دوره:", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(courseName) || studentSectionIndex == -1 || studentSectionIndex + 1 >= lines.Length)
            {
                await _bot.SendMessage(message.Chat.Id, "❌ فرمت پیام اشتباه است. لطفاً از ساختار زیر استفاده کنید:\n\n" +
                    "/edit-course\nاسم دوره: مثال\nدانشجو های دوره:\n- علی\n- زهرا");
                return;
            }

            var existingCourse = await _courseRepository.GetByNameAsync(courseName);
            if (existingCourse == null)
            {
                await _bot.SendMessage(message.Chat.Id, $"❌ دوره‌ای با نام «{courseName}» یافت نشد.");
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

            var newStudents = studentLines
                .Select(name => new Student(name, existingCourse.Id))
                .ToList();

            await _courseRepository.UpdateStudentsAsync(existingCourse, newStudents);

            await _bot.SendMessage(
                message.Chat.Id,
                $"✅ دوره «{courseName}» با {newStudents.Count} دانشجو با موفقیت بروزرسانی شد. 🔄"
            );
        }
    }
}
