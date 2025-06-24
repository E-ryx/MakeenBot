using System.Text.RegularExpressions;
using MakeenBot.Models;

namespace MakeenBot.Validators;

public class ProccesReport
{
    public string? ErrorMessage { get; private set; }

    public bool IsValidReport(string text, out DailyReport? report)
    {
        report = new DailyReport();

        // Validate Persian date
        var dateMatch = Regex.Match(text, @"تاریخ:\s*(\d{2}/\d{2}/\d{4})");
        if (!dateMatch.Success)
        {
            ErrorMessage = "❌ تاریخ به درستی وارد نشده است. فرمت صحیح: 01/01/1403";
            return false;
        }
        report.PersianDate = dateMatch.Groups[1].Value;

        // Validate name tag
        var nameTagMatch = Regex.Match(text, @"نام و نام خانوادگی:\s*(#([\u0600-\u06FF_]+))");
        if (!nameTagMatch.Success)
        {
            ErrorMessage = "❌ نام و نام خانوادگی به درستی وارد نشده است. باید با # و حروف فارسی باشد.";
            return false;
        }
        report.NameTag = nameTagMatch.Groups[2].Value;

        // Validate report number
        var reportNumberMatch = Regex.Match(text, @"شماره گزارش:\s*(\d+)");
        if (!reportNumberMatch.Success || !int.TryParse(reportNumberMatch.Groups[1].Value, out int reportNumber))
        {
            ErrorMessage = "❌ شماره گزارش نامعتبر است.";
            return false;
        }
        report.ReportNumber = reportNumber;

        // Validate work hour
        var workHourMatch = Regex.Match(text, @"مجموع ساعت:\s*(\d+)");
        if (!workHourMatch.Success || !int.TryParse(workHourMatch.Groups[1].Value, out int workHour))
        {
            ErrorMessage = "❌ مجموع ساعت کار نامعتبر است.";
            return false;
        }
        report.WorkHour = workHour;

        // All validations passed
        ErrorMessage = null;
        return true;
    }
}
