using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Response
{
    public class MessageDto
    {
        [JsonProperty("message_id")]
        public long MessageId { get; set; }

        [JsonProperty("from")]
        public UserDto From { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }

        [JsonProperty("chat")]
        public ChatDto Chat { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("entities")]
        public MessageEntityDto[] Entities { get; set; }
    }
}
