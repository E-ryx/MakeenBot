using System;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Interfaces.Validators;

public interface IReportValidator
{
    ValidatorErrorMessage ValidateReport(string report);
}
