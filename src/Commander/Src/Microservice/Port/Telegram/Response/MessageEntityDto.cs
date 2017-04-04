using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Response
{
    public class MessageEntityDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("offset")]
        public short Offset { get; set; }

        [JsonProperty("length")]
        public short Length { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("user")]
        public UserDto User { get; set; }
    }
}
