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
        private readonly BotDbContext _context;

        public ReportRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task SaveReportAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
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
    }
}