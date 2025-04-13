using MakeenBot.Models;

namespace MakeenBot.Interfaces
{
    public interface IReportService
    {
        public DailyReport? ParseDailyReport(string text);
    }
}
