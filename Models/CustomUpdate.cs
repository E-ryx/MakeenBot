using Newtonsoft.Json;

public class CustomUpdate
{
    [JsonProperty("update_id")]
    public int UpdateId { get; set; }

    [JsonProperty("message")]
    public CustomMessage Message { get; set; }
}