using ClosedXML.Excel;
using MakeenBot.Extensions;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Interfaces.Services;
using MakeenBot.Interfaces.Validators;
using MakeenBot.Models.Entities;
using MakeenBot.Models.ValueObjects;
using System.Globalization;
using System.Text;

namespace MakeenBot.Services;

public class ReportService : IReportService
{
    private readonly IReportValidator _validator;
    private readonly IStudentRepository _studentRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ICourseRepository _courseRepository;

    public ReportService(
        IReportValidator validator,
        IStudentRepository studentRepository,
        IReportRepository reportRepository,
        ICourseRepository courseRepository)
    {
        _validator = validator;
        _studentRepository = studentRepository;
        _reportRepository = reportRepository;
        _courseRepository = courseRepository;
    }
    public async Task<(bool IsSuccess, string Message)> HandleReportSubmissionAsync(string messageText, bool isEdit = false)
    {
        var (reportDto, errors) = await _validator.ValidateAndParseAsync(messageText, isEdit);
        if (errors.Any())
            return (false, string.Join("\n", errors));

        var student = await _studentRepository.GetByNameAsync(reportDto!.StudentName);
        if (student == null)
            return (false, "❌ دانشجویی با این نام پیدا نشد.");

        var existingReport = await _reportRepository.GetReportByStudentIdAndNumberAsync(student.Id, reportDto.ReportNumber);

        if (isEdit)
        {
            if (existingReport == null)
                return (false, "❌ گزارشی برای ویرایش یافت نشد.");

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

    public async Task<MemoryStream?> ExportReportsToExcelAsync(string courseName, DateTime startDate, DateTime endDate)
    {
        var course = await _courseRepository.GetByNameAsync(courseName);
        if (course == null)
            return null;

        var students = course.Students.OrderBy(s => s.Name).ToList();
        var reports = await _reportRepository.GetReportsByCourseAndDateRangeAsync(course.Name, startDate, endDate);

        var reportInfoList = reports
            .GroupBy(r => r.ReportNumber)
            .Select(g => new
            {
                ReportNumber = g.Key,
                FirstDate = g.First().Date
            })
            .OrderBy(x => x.ReportNumber)
            .ToList();

        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("گزارش‌ها");

        // Header: شماره گزارش و تاریخ شمسی
        worksheet.Cell(1, 1).Value = "نام دانشجو";
        worksheet.Cell(2, 1).Value = "";

        int colIndex = 2;
        foreach (var reportInfo in reportInfoList)
        {
            var persianDate = reportInfo.FirstDate.ToPersianDateTextify();
            worksheet.Cell(1, colIndex).Value = $"{reportInfo.ReportNumber}";
            worksheet.Cell(2, colIndex).Value = persianDate;
            colIndex++;
        }

        int rowIndex = 3;
        foreach (var student in students)
        {
            worksheet.Cell(rowIndex, 1).Value = student.Name;
            int currentCol = 2;

            foreach (var reportInfo in reportInfoList)
            {
                var reportNumber = reportInfo.ReportNumber;
                var report = reports.FirstOrDefault(r => r.StudentId == student.Id && r.ReportNumber == reportNumber);

                var cell = worksheet.Cell(rowIndex, currentCol);
                if (report != null)
                {
                    cell.Value = report.Hours;
                    if (report.IsFailed)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }
                }
                else
                {
                    cell.Value = "-";
                    cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                currentCol++;
            }

            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }
}
