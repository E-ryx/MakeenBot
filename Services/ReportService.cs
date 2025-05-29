using MakeenBot.Models;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using MakeenBot.Interfaces;
using MakeenBot.Interfaces.Services;

namespace MakeenBot.Services
{
    public class ReportService : IReportService
    {
        public DailyReport? ParseDailyReport(string text)
        {
            var report = new DailyReport();

            report.PersianDate = ExtractMatch(text, @"تاریخ:\s*(\d{2}/\d{2}/\d{4})", 1);
            report.NameTag = ExtractMatch(text, @"نام و نام خانوادگی:\s*(#([\u0600-\u06FF_]+))", 2);
            report.ReportNumber = ExtractIntMatch(text, @"شماره گزارش:\s*(\d+)");
            report.WorkHour = ExtractIntMatch(text, @"مجموع ساعت:\s*(\d+)");

            return IsReportValid(report) ? report : null;
        }

        private string? ExtractMatch(string text, string pattern, int groupIndex)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[groupIndex].Value.Trim() : null;
        }

        private int? ExtractIntMatch(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success && int.TryParse(match.Groups[1].Value, out int number) ? number : (int?)null;
        }

        private bool IsReportValid(DailyReport report)
        {
            return !string.IsNullOrEmpty(report.PersianDate) &&
                   !string.IsNullOrEmpty(report.NameTag) &&
                   report.ReportNumber.HasValue &&
                   report.WorkHour.HasValue;
        }
    }
}
