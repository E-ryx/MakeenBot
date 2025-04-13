using MakeenBot.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using MakeenBot.Interfaces;

namespace MakeenBot.Services
{
    public class ReportService: IReportService
    {
        public DailyReport? ParseDailyReport(string text)
{
    var report = new DailyReport();

    var dateMatch = Regex.Match(text, @"تاریخ:\s*(\d{2}/\d{2}/\d{4})");
    if (dateMatch.Success)
        report.PersianDate = dateMatch.Groups[1].Value.Trim();

    var nameMatch = Regex.Match(text, @"نام و نام خانوادگی:\s*(#([\u0600-\u06FF_]+))");
    if (nameMatch.Success)
        report.NameTag = nameMatch.Groups[2].Value;

    var reportNumMatch = Regex.Match(text, @"شماره گزارش:\s*(\d+)");
    if (reportNumMatch.Success && int.TryParse(reportNumMatch.Groups[1].Value, out int number))
        report.ReportNumber = number;

    var totalHoursMatch = Regex.Match(text, @"مجموع ساعت:\s*(\d+)");
    if (totalHoursMatch.Success && int.TryParse(totalHoursMatch.Groups[1].Value, out int totalHours))
        report.WorkHour = totalHours; // Assuming you've added this to your model

    if (!string.IsNullOrEmpty(report.PersianDate)
        && !string.IsNullOrEmpty(report.NameTag)
        && report.ReportNumber.HasValue
        && report.WorkHour.HasValue)
    {
        return report;
    }

    return null;
}


    }
}
