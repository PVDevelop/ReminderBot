using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Response
{
    public class ResponseDto<TResult>
    {
        [JsonProperty("ok")]
        public bool IsOk { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("result")]
        public TResult Result { get; set; }
    }
}
