using System.Text.RegularExpressions;
using MakeenBot.Interfaces.Validators;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Validators;

public class ReportValidator : IReportValidator
{
    public ValidatorErrorMessage ValidateReport(string report)
    {
        // Match and convert Persian date
        var dateMatch = Regex.Match(report, @"تاریخ:\s*([\d۰-۹]{2}/[\d۰-۹]{2}/[\d۰-۹]{4})");
        if (!dateMatch.Success)
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ تاریخ به درستی وارد نشده است. فرمت صحیح: 01/01/1403",
                IsValid = false
            };
        }
        var persianDate = ConvertPersianDigitsToEnglish(dateMatch.Groups[1].Value);

        // Match and extract name
        var nameMatch = Regex.Match(report, @"نام و نام خانوادگی:\s*#([\u0600-\u06FF_]+)");
        if (!nameMatch.Success)
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ نام و نام خانوادگی به درستی وارد نشده است. باید با # و حروف فارسی باشد.",
                IsValid = false
            };
        }

        // Match and convert report number
        var reportNumberMatch = Regex.Match(report, @"شماره گزارش:\s*([\d۰-۹]+)");
        if (!reportNumberMatch.Success)
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ شماره گزارش به درستی وارد نشده است.",
                IsValid = false
            };
        }
        var reportNumberStr = ConvertPersianDigitsToEnglish(reportNumberMatch.Groups[1].Value);
        if (!int.TryParse(reportNumberStr, out int reportNumber))
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ شماره گزارش باید یک عدد معتبر باشد.",
                IsValid = false
            };
        }

        // Match and convert work hour
        var workHourMatch = Regex.Match(report, @"مجموع ساعت:\s*([\d۰-۹]+)");
        if (!workHourMatch.Success)
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ مجموع ساعت کار به درستی وارد نشده است.",
                IsValid = false
            };
        }
        var workHourStr = ConvertPersianDigitsToEnglish(workHourMatch.Groups[1].Value);
        if (!int.TryParse(workHourStr, out int workHour))
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ مجموع ساعت کار باید یک عدد معتبر باشد.",
                IsValid = false
            };
        }

        // All valid — build DTO
        var dto = new ReportDto
        {
            Date = persianDate,
            StudentName = nameMatch.Groups[1].Value,
            ReportNumber = reportNumber,
            WorkHour = workHour
        };

        return new ValidatorErrorMessage
        {
            IsValid = true,
        };
    }

    private string ConvertPersianDigitsToEnglish(string input)
    {
        return input
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
    }

}