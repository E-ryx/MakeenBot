using MakeenBot.Models;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Interfaces
{
    public interface IReportRepository
    {
        Task<OperationResult> SaveReportAsync(DailyReport report);
    }
}
