using System;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace MakeenBot.Services;

public class BotService
{
public Update ProcessUpdate(string json)
    {
        try
        {
            // Step 1: Replace first_name with FirstName to match library expectations
            json = json.Replace("\"first_name\":", "\"FirstName\":");
            json = json.Replace("\"last_name\":", "\"LastName\":");
            json = json.Replace("\"is_bot\":", "\"IsBot\":");
            json = json.Replace("\"message_id\":", "\"MessageId\":");
            json = json.Replace("\"update_id\":", "\"Id\":");
            json = json.Replace("\"chat\":", "\"Chat\":");
            json = json.Replace("\"text\":", "\"Text\":");
            json = json.Replace("\"from\":", "\"From\":");
            json = json.Replace("\"date\":", "\"Date\":");
            json = json.Replace("\"username\":", "\"Username\":");
            json = json.Replace("\"id\":", "\"Id\":");

            // Step 2: Deserialize directly into Telegram.Bot's Update
            var update = JsonConvert.DeserializeObject<Update>(json);

            return update;

            // Pass to Telegram.Bot's processing logic
            // e.g., botClient.OnUpdateReceived(update);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing update: {ex.Message}");
            throw;
        }
    }
}
