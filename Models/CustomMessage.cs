using Newtonsoft.Json;
using Telegram.Bot.Types;

public class CustomMessage
{
    [JsonProperty("message_id")]
    public int MessageId { get; set; }

    [JsonProperty("from")]
    public CustomUser From { get; set; }

    [JsonProperty("date")]
    public long Date { get; set; }

    [JsonProperty("chat")]
    public Chat Chat { get; set; } // Use Telegram.Bot's Chat class or create a custom one

    [JsonProperty("text")]
    public string Text { get; set; }
}
