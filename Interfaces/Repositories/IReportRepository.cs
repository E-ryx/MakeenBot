using MakeenBot.Models;
using MakeenBot.Models.Entities;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task SaveReportAsync(Report report);
        //Task<bool> ExistsByReportNumberAsync(int reportNumber);
        //Task<bool> ExistsOtherNumberOnSameDateAsync(DateTime date, int currentReportNumber);
        Task UpdateReportAsync(Report report);
        Task<Report?> GetReportByStudentIdAndNumberAsync(int studentId, int reportNumber);
    Task<List<Report>> GetReportsByCourseAndDateRangeAsync(string courseName, DateTime startDate, DateTime endDate);
        Task<bool> ExistsByReportNumberForStudentAsync(int studentId, int reportNumber);
        Task<bool> ExistsOtherNumberOnSameDateForStudentAsync(int studentId, DateTime date, int reportNumber);
        //Task SaveAsync(ReportDto report);
    }
}
