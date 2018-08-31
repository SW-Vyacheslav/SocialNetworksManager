using System;

using Newtonsoft.Json;

namespace SlackExtension.Responses
{
    public class SlackChannelsListResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("channels")]
        public Models.SlackChannel[] Channels { get; set; }

        [JsonProperty("error")]
        public String Error { get; set; }
    }
}