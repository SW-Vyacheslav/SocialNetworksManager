using System;

using Newtonsoft.Json;

namespace SlackExtension.Models
{
    public class SlackPaging
    {
        [JsonProperty("count")]
        public UInt64 Count { get; set; }

        [JsonProperty("total")]
        public UInt64 Total { get; set; }

        [JsonProperty("page")]
        public UInt64 Page { get; set; }

        [JsonProperty("pages")]
        public UInt64 Pages { get; set; }
    }
}
