using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackExtension.Models
{
    public class SlackChannel
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("is_channel")]
        public Boolean IsChannel { get; set; }

        [JsonProperty("created")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Created { get; set; }

        [JsonProperty("creator")]
        public String Creator { get; set; }

        [JsonProperty("is_archived")]
        public Boolean IsArchived { get; set; }

        [JsonProperty("is_general")]
        public Boolean IsGeneral { get; set; }

        [JsonProperty("name_normalied")]
        public String NameNormalized { get; set; }

        [JsonProperty("is_shared")]
        public Boolean IsShared { get; set; }

        [JsonProperty("is_org_shared")]
        public Boolean IsOrgShared { get; set; }

        [JsonProperty("is_member")]
        public Boolean IsMember { get; set; }

        [JsonProperty("is_private")]
        public Boolean IsPrivate { get; set; }

        [JsonProperty("is_mpim")]
        public Boolean IsMPIM { get; set; }

        [JsonProperty("last_read")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastRead { get; set; }

        [JsonProperty("unread_count")]
        public UInt64 UnreadCount { get; set; }

        [JsonProperty("unread_count_display")]
        public UInt64 UnreadCountDisplay { get; set; }

        [JsonProperty("members")]
        public String[] Members { get; set; }

        [JsonProperty("topic")]
        public Models.SlackTopicInformation Topic { get; set; }

        [JsonProperty("purpose")]
        public Models.SlackPurposeInformation Purpose { get; set; }

        [JsonProperty("previous_names")]
        public String[] PreviousNames { get; set; }

        [JsonProperty("num_members")]
        public UInt64 NumMembers { get; set; }
    }
}