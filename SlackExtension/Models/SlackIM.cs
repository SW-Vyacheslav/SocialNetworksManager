using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackExtension.Models
{
    public class SlackIM
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("is_im")]
        public Boolean IsIM { get; set; }

        [JsonProperty("user")]
        public String User { get; set; }

        [JsonProperty("created")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Created { get; set; }

        [JsonProperty("is_user_deleted")]
        public Boolean IsUserDeleted { get; set; }
    }
}
