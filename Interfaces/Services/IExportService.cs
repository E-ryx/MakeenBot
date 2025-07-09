namespace MakeenBot.Interfaces.Services
{
    public interface IExportService
    {
        Task<MemoryStream?> ExportReportsToExcelAsync(string courseName, DateTime startDate, DateTime endDate);
    }
}
