using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackExtension.Models
{
    public class SlackPurposeInformation
    {
        [JsonProperty("value")]
        public String Value { get; set; }

        [JsonProperty("creator")]
        public String Creator { get; set; }

        [JsonProperty("last_set")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSet { get; set; }
    }
}
