using MakeenBot.Interfaces.Repositories;
using MakeenBot.Interfaces.Validators;
using MakeenBot.Models.ValueObjects;
using System.Text.RegularExpressions;

public class ReportValidator : IReportValidator
{
    private readonly IStudentRepository _studentRepository;
    private readonly IReportRepository _reportRepository;

    public ReportValidator(IStudentRepository studentRepository, IReportRepository reportRepository)
    {
        _studentRepository = studentRepository;
        _reportRepository = reportRepository;
    }

    public async Task<(ReportDto? Report, List<string> Errors)> ValidateAndParseAsync(string input, bool isEdit = false)
    {
        var errors = new List<string>();
        var report = new ReportDto
        {
            Date = DateTime.Now
        };

        var nameMatch = Regex.Match(input, @"نام و نام خانوادگی:\s*#([\u0600-\u06FF_]+)");
        if (!nameMatch.Success)
        {
            errors.Add("❌ نام و نام خانوادگی به درستی وارد نشده است.");
            return (null, errors);
        }

        var rawName = nameMatch.Groups[1].Value.Trim();
        report.StudentName = rawName.Replace("_", " ").Trim();

        var student = await _studentRepository.GetByNameAsync(report.StudentName); if (student == null)
        {
            errors.Add("❌ دانشجویی با این نام پیدا نشد.");
            return (null, errors);
        }

        var numberMatch = Regex.Match(input, @"شماره گزارش:\s*([\d۰-۹0-9]+)");
        if (!numberMatch.Success)
            errors.Add("❌ شماره گزارش به درستی وارد نشده است.");
        else
        {
            var number = ConvertPersianDigitsToEnglishInt(numberMatch.Groups[1].Value);
            if (number == null)
                errors.Add("❌ شماره گزارش باید عدد باشد.");
            else
            {
                report.ReportNumber = number.Value;

                if (!isEdit)
                {
                    if (await _reportRepository.ExistsByReportNumberForStudentAsync(student.Id, number.Value))
                        errors.Add("❌ این شماره گزارش قبلاً توسط شما ثبت شده است.");

                    if (await _reportRepository.ExistsOtherNumberOnSameDateForStudentAsync(student.Id, report.Date, number.Value))
                        errors.Add("❌ شما در این تاریخ گزارشی با شماره‌ی دیگری ثبت کرده‌اید.");
                }
            }
        }

        var hourMatch = Regex.Match(input, @"مجموع ساعت:\s*([\d۰-۹0-9]+)");
        if (!hourMatch.Success)
            errors.Add("❌ مجموع ساعت به درستی وارد نشده است.");
        else
        {
            var hour = ConvertPersianDigitsToEnglishInt(hourMatch.Groups[1].Value);
            if (hour == null)
                errors.Add("❌ مجموع ساعت باید عدد باشد.");
            else if (hour > 24)
                errors.Add("❌ مجموع ساعت نباید بیشتر از ۲۴ باشد.");
            else
                report.WorkHour = hour.Value;
        }

        return errors.Any() ? (null, errors) : (report, errors);
    }


    private int? ConvertPersianDigitsToEnglishInt(string input)
    {
        string englishDigits = input
            .Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3")
            .Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7")
            .Replace("۸", "8").Replace("۹", "9");

        return int.TryParse(englishDigits, out int result) ? result : (int?)null;
    }
}
