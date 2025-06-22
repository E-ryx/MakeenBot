using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MakeenBot.Models.ValueObjects;

namespace MakeenBot.Validators;

public class ReportValidator
{
    public ValidatorErrorMessage AddReport(string report)
    {
        // Validate Persian date
        if (!Regex.IsMatch(report, @"تاریخ:\s*\d{2}/\d{2}/\d{4}"))
        {
            return new ValidatorErrorMessage {
                ErrorMessage = "❌ تاریخ به درستی وارد نشده است. فرمت صحیح: 01/01/1403",
                IsValid = false
            };
        }

        if (!Regex.IsMatch(report, @"نام و نام خانوادگی:\s*#([\u0600-\u06FF_]+)"))
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ نام و نام خانوادگی به درستی وارد نشده است. باید با # و حروف فارسی باشد.",
                IsValid = false
            };
        }

        if (!Regex.IsMatch(report, @"شماره گزارش:\s*\d+"))
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ شماره گزارش به درستی وارد نشده است.",
                IsValid = false
            };
        }

        if (!Regex.IsMatch(report, @"مجموع ساعت:\s*\d+"))
        {
            return new ValidatorErrorMessage
            {
                ErrorMessage = "❌ مجموع ساعت کار به درستی وارد نشده است.",
                IsValid = false
            };
        }

        return new ValidatorErrorMessage
        {
            IsValid = true,
        };
    }

}
