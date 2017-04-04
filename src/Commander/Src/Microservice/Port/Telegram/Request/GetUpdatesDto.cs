using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Request
{
    public class GetUpdatesDto
    {
        [JsonProperty("offset")]
        public long Offset { get; }

        [JsonProperty("limit")]
        public long Limit { get; }

        [JsonProperty("timeout")]
        public long Timeout { get; }

        [JsonProperty("allowed_updates")]
        public string[] AllowedUpdates { get; }

        public GetUpdatesDto(
            long offset,
            long limit,
            long timeout = 0,
            string[] allowedUpdates = null)
        {
            Offset = offset;
            Limit = limit;
            Timeout = timeout;
            AllowedUpdates = allowedUpdates ?? new string[0];
        }
    }
}
