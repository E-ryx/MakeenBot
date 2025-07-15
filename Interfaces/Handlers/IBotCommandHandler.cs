using Telegram.Bot.Types;

namespace MakeenBot.Interfaces.Handlers
{
    public interface IBotCommandHandler
    {
        bool CanHandle(string messageText);
        Task HandleAsync(Message message);
    }
}
