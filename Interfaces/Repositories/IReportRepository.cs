using MakeenBot.Models;

namespace MakeenBot.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<OperationResult> SaveReportAsync(DailyReport report);
    }
}
