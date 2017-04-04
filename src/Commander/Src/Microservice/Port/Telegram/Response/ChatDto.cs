using Newtonsoft.Json;

namespace PVDevelop.ReminderBot.Microservice.Response
{
    public class ChatDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("all_members_are_administrators")]
        public bool AreAllMembersAdmins { get; set; }
    }
}
