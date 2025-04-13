using MakeenBot.Models;

namespace MakeenBot.Interfaces
{
    public interface IReportRepository
    {
        Task<OperationResult> SaveReportAsync(DailyReport report);
    }
}
