using MakeenBot.Models;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using MakeenBot.Interfaces;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Services
{
    public class ReportService : IReportService
    {
        public ReportDto? ParseDailyReport(string text)
        {
            var report = new ReportDto();

            report.Date = ExtractMatch(text, @"تاریخ:\s*(\d{2}/\d{2}/\d{4})", 1);
            report.StudentName = ExtractMatch(text, @"نام و نام خانوادگی:\s*(#([\u0600-\u06FF_]+))", 2);
            report.ReportNumber = ConvertPersianDigitsToEnglish(ExtractMatch(text, @"شماره گزارش:\s*(\d+)", 3));
            report.WorkHour = ConvertPersianDigitsToEnglish(ExtractMatch(text, @"مجموع ساعت:\s*(\d+)", 4));

            return IsReportValid(report) ? report : null;
        }

        private string? ExtractMatch(string text, string pattern, int groupIndex)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[groupIndex].Value.Trim() : null;
        }

        private bool IsReportValid(ReportDto report)
        {
            return !string.IsNullOrEmpty(report.Date) &&
                   !string.IsNullOrEmpty(report.StudentName) &&
                   report.ReportNumber.HasValue &&
                   report.WorkHour.HasValue;
        }
        private int? ConvertPersianDigitsToEnglish(string input)
    {
        string englishDigits = input
        .Replace("۰", "0")
        .Replace("۱", "1")
        .Replace("۲", "2")
        .Replace("۳", "3")
        .Replace("۴", "4")
        .Replace("۵", "5")
        .Replace("۶", "6")
        .Replace("۷", "7")
        .Replace("۸", "8")
        .Replace("۹", "9");

    return int.TryParse(englishDigits, out int result) ? result : (int?)null;
    }
    }
}
