using MakeenBot.Models;

namespace MakeenBot.Interfaces.Services
{
    public interface IReportService
    {
        Task<MemoryStream?> ExportReportsToExcelAsync(string courseName, DateTime startDate, DateTime endDate);
        Task<(bool IsSuccess, string Message)> HandleReportSubmissionAsync(string messageText, bool isEdit = false);
    }
}
