using System;

namespace MakeenBot.Models.ValueObjects;

public class BotConfig
{
    public string Token { get; set; }
    public string BaleApi { get; set; }
    public string Webhook { get; set; }
}
