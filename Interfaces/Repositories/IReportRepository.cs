using MakeenBot.Models;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<OperationResult> SaveReportAsync(DailyReport report);
    }
}
