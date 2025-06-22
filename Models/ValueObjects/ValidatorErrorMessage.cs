using System;

namespace MakeenBot.Models.ValueObjects;

public class ValidatorErrorMessage
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}
