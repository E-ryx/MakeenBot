using System;
using MakeenBot.Models;
using MakeenBot.Interfaces;
using ClosedXML.Excel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MakeenBot.Models.ValueObjects;
using MakeenBot.Models.Entities;
using MakeenBot.Data;
using Microsoft.EntityFrameworkCore;
using MakeenBot.Interfaces.Repositories;

namespace MakeenBot.Repositories
{
    public class ReportRepository : IReportRepository
    {
        //private readonly string _folderPath = "Reports";
        private readonly BotDbContext _context;

        public ReportRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task SaveReportAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync(); // 👈 این خط رو اضافه کن
        }
        public async Task UpdateReportAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }
        public async Task<Report?> GetReportByStudentIdAndNumberAsync(int studentId, int reportNumber)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(r =>
                    r.StudentId == studentId &&
                    r.ReportNumber == reportNumber);
        }
public async Task<List<Report>> GetReportsByCourseAndDateRangeAsync(string courseName, DateTime startDate, DateTime endDate)
    {
        return await _context.Reports
            .AsNoTracking()
            .Include(r => r.Student)
            .ThenInclude(s => s.Course)
            .Where(r => r.Student.Course.Name == courseName &&
                        r.Date.Date >= startDate.Date &&
                        r.Date.Date <= endDate.Date)
            .ToListAsync();
    }

        public async Task<bool> ExistsByReportNumberForStudentAsync(int studentId, int reportNumber)
        {
            return await _context.Reports.AnyAsync(r =>
                r.StudentId == studentId &&
                r.ReportNumber == reportNumber);
        }

        public async Task<bool> ExistsOtherNumberOnSameDateForStudentAsync(int studentId, DateTime date, int reportNumber)
        {
            return await _context.Reports.AnyAsync(r =>
                r.StudentId == studentId &&
                r.Date.Date == date.Date &&
                r.ReportNumber != reportNumber);
        }
        //public async Task<OperationResult> SaveReportAsync(DailyReport report)
        //{
        //    // Ensure the directory exists
        //    Directory.CreateDirectory(_folderPath);

        //    // Use user's name as the file name
        //    var sanitizedFileName = $"{report.NameTag}.xlsx";
        //    var filePath = Path.Combine(_folderPath, sanitizedFileName);

        //    XLWorkbook workbook;

        //    // Load existing file or create new
        //    if (File.Exists(filePath))
        //    {
        //        workbook = new XLWorkbook(filePath);
        //        var existingWorksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Reports");

        //        // Check if report number or date already exists
        //        if (existingWorksheet.RowsUsed().Any(row =>
        //            row.Cell(3).Value.ToString() == report.ReportNumber.ToString() &&
        //            row.Cell(2).Value.ToString() == report.NameTag) ||
        //            existingWorksheet.RowsUsed().Any(row =>
        //            row.Cell(1).Value.ToString() == report.PersianDate.ToString() &&
        //            row.Cell(2).Value.ToString() == report.NameTag))
        //        {
        //            // Report number or date already exists for this user, don't save
        //            return OperationResult.Fail($"{report.NameTag.Replace("_", " ")} عزیز\n⚠️ گزارشی با شماره {report.ReportNumber} یا تاریخ {report.PersianDate} قبلا ثبت شده است.\n لطفا گزارش جدید بفرستید.");
        //        }
        //    }
        //    else
        //    {
        //        workbook = new XLWorkbook();
        //    }

        //    var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Reports");

        //    // Add headers if the file is new or empty
        //    if (worksheet.FirstRowUsed() == null)
        //    {
        //        worksheet.Cell(1, 3).Value = "Report Number";
        //        worksheet.Cell(1, 1).Value = "Date";
        //        worksheet.Cell(1, 2).Value = "Name";
        //        worksheet.Cell(1, 4).Value = "Work Hours";
        //    }

        //    // Find next empty row
        //    var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        //    var nextRow = lastRow + 1;

        //    worksheet.Cell(nextRow, 3).Value = report.ReportNumber;
        //    worksheet.Cell(nextRow, 1).Value = report.PersianDate;
        //    worksheet.Cell(nextRow, 2).Value = report.NameTag;
        //    worksheet.Cell(nextRow, 4).Value = report.WorkHour;

        //    workbook.SaveAs(filePath);

        //    return OperationResult.Ok($"{report.NameTag.Replace("_", " ")} عزیز\n✅ گزارش {report.ReportNumber} با موفقیت ذخیره شد.");
        //}
    }
}