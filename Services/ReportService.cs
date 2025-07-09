using MakeenBot.Models.ValueObjects;
using MakeenBot.Interfaces.Validators;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Models.Entities;
using MakeenBot.Interfaces.Services;

namespace MakeenBot.Services;

public class ReportService : IReportService
{
    private readonly IReportValidator _validator;
    private readonly IStudentRepository _studentRepository;
    private readonly IReportRepository _reportRepository;

    public ReportService(
        IReportValidator validator,
        IStudentRepository studentRepository,
        IReportRepository reportRepository)
    {
        _validator = validator;
        _studentRepository = studentRepository;
        _reportRepository = reportRepository;
    }

    public async Task<(bool IsSuccess, string Message)> HandleReportSubmissionAsync(string messageText, bool isEdit = false)
    {
        var (reportDto, errors) = await _validator.ValidateAndParseAsync(messageText, isEdit);
        if (errors.Any())
            return (false, string.Join("\n", errors));

        var student = await _studentRepository.GetByNameAsync("#" + reportDto!.StudentName);
        if (student == null)
            return (false, "❌ دانشجویی با این نام پیدا نشد.");

        var existingReport = await _reportRepository.GetReportByStudentIdAndNumberAsync(student.Id, reportDto.ReportNumber);

        if (isEdit)
        {
            if (existingReport == null)
                return (false, "❌ گزارشی برای ویرایش یافت نشد.");

            // فقط اگر برای همون روز باشه قابل ویرایشه
            if (existingReport.Date.Date != DateTime.Now.Date)
                return (false, "❌ فقط گزارش‌های امروز قابل ویرایش هستند.");

            existingReport.Update(reportDto.WorkHour);

            await _reportRepository.UpdateReportAsync(existingReport);

            var editMsg = $"✏️ گزارش شما با موفقیت ویرایش شد.";
            if (existingReport.IsFailed)
                editMsg += "\n⚠️ این ویرایش بعد از ساعت ۱۰ شب ثبت شده و به عنوان گزارش دیرهنگام علامت‌گذاری شد.";

            return (true, editMsg);
        }
        else
        {
            // ثبت جدید
            var newReport = new Report(
                reportNumber: reportDto.ReportNumber,
                hours: reportDto.WorkHour,
                studentId: student.Id
            );

            await _reportRepository.SaveReportAsync(newReport);

            var msg = $"✅ {student.Name} عزیز، گزارش شما با موفقیت ثبت شد.";
            if (newReport.IsFailed)
                msg += "\n⚠️ این گزارش بعد از ساعت ۱۰ شب ارسال شده و به عنوان گزارش دیرهنگام علامت‌گذاری شد.";

            return (true, msg);
        }
    }

}
