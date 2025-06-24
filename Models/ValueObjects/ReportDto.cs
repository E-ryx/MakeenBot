using System;

namespace MakeenBot.Models.ValueObjects;

public class ReportDto
{
    public string StudentName { get; set; }
    public string Date { get; set; }
    public int ReportNumber { get; set; }
    public int WorkHour { get; set; }
}
