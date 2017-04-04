using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Application.Commands
{
    public class PrintMessageCommand
    {
        [JsonProperty("chat_id")]
        public long ChatId { get; }

        [JsonProperty("text")]
        public string Text { get; }

        public PrintMessageCommand(long chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }
    }
}
