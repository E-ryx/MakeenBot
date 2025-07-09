using MakeenBot.Models;

namespace MakeenBot.Interfaces.Services
{
    public interface IReportService
    {
        Task<(bool IsSuccess, string Message)> HandleReportSubmissionAsync(string messageText, bool isEdit = false);

    }
}
