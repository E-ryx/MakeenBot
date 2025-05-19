using MakeenBot.Models;

namespace MakeenBot.Interfaces.Services
{
    public interface IReportService
    {
        public DailyReport? ParseDailyReport(string text);
    }
}
