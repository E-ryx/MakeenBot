using System;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Interfaces.Validators;

public interface IReportValidator
{
    Task<(ReportDto? Report, List<string> Errors)> ValidateAndParseAsync(string input, bool isEdit = false);
}
