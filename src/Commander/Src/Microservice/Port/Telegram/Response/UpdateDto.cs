using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Response
{
    public class UpdateDto
    {
        [JsonProperty("update_id")]
        public long UpdateId { get; set; }

        [JsonProperty("message")]
        public MessageDto Message { get; set; }
    }
}
