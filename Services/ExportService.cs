using ClosedXML.Excel;
using MakeenBot.Extensions;
using MakeenBot.Interfaces.Repositories;
using MakeenBot.Interfaces.Services;
using MakeenBot.Models.Entities;
using System.Globalization;
using System.Text;

namespace MakeenBot.Services
{
    public class ExportService : IExportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICourseRepository _courseRepository;

        public ExportService(IReportRepository reportRepository, ICourseRepository courseRepository)
        {
            _reportRepository = reportRepository;
            _courseRepository = courseRepository;
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
                var persianDate = reportInfo.FirstDate.ToPersianDateTextify(); // تبدیل به تاریخ شمسی
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

            // LINQ Version

            //// 1. ساخت ستون‌ها (شماره گزارش + تاریخ شمسی)
            //var columnHeaders = reportInfoList
            //    .Select((info, index) => new
            //    {
            //        Column = index + 2, // از ستون دوم شروع می‌کنیم
            //        ReportNumber = info.ReportNumber,
            //        PersianDate = info.FirstDate.ToPersianDateTextify()
            //    })
            //    .ToList();

            //foreach (var header in columnHeaders)
            //{
            //    worksheet.Cell(1, header.Column).Value = header.ReportNumber.ToString();
            //    worksheet.Cell(2, header.Column).Value = header.PersianDate;
            //}

            //// 2. ساخت ردیف‌ها (نام دانشجو + گزارشات)
            //var studentRows = students
            //    .Select((student, rowIndex) => new
            //    {
            //        Row = rowIndex + 3,
            //        Student = student,
            //        Reports = columnHeaders.Select(header =>
            //        {
            //            var report = reports.FirstOrDefault(r => r.StudentId == student.Id && r.ReportNumber == header.ReportNumber);
            //            return new
            //            {
            //                Column = header.Column,
            //                Hours = report?.Hours,
            //                IsFailed = report?.IsFailed ?? false,
            //                HasReport = report != null
            //            };
            //        }).ToList()
            //    })
            //    .ToList();

            //foreach (var row in studentRows)
            //{
            //    worksheet.Cell(row.Row, 1).Value = row.Student.Name;

            //    foreach (var reportCell in row.Reports)
            //    {
            //        var cell = worksheet.Cell(row.Row, reportCell.Column);

            //        if (reportCell.HasReport)
            //        {
            //            cell.Value = reportCell.Hours;
            //            if (reportCell.IsFailed)
            //                cell.Style.Fill.BackgroundColor = XLColor.Yellow;
            //        }
            //        else
            //        {
            //            cell.Value = "-";
            //            cell.Style.Fill.BackgroundColor = XLColor.LightPink;
            //            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //        }
            //    }
            //}

        }
    }
}
