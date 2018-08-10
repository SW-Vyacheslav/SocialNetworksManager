using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackExtension.Models
{
    public class SlackUser
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("team_id")]
        public String TeamID { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("deleted")]
        public Boolean Deleted { get; set; }

        [JsonProperty("color")]
        public String Color { get; set; }

        [JsonProperty("real_name")]
        public String RealName { get; set; }

        [JsonProperty("tz")]
        public String TimeZone { get; set; }

        [JsonProperty("tz_label")]
        public String TimeZoneLabel { get; set; }

        [JsonProperty("tz_offset")]
        public Int64 TimeZoneOffset { get; set; }

        [JsonProperty("profile")]
        public UserProfile Profile { get; set; }

        [JsonProperty("is_admin")]
        public Boolean IsAdmin { get; set; }

        [JsonProperty("is_owner")]
        public Boolean IsOwner { get; set; }

        [JsonProperty("is_primary_owner")]
        public Boolean IsPrimaryOwner { get; set; }

        [JsonProperty("is_restricted")]
        public Boolean IsRestricted { get; set; }

        [JsonProperty("is_ultra_restricted")]
        public Boolean IsUltraRestricted { get; set; }

        [JsonProperty("is_bot")]
        public Boolean IsBot { get; set; }

        [JsonProperty("is_stranger")]
        public Boolean IsStranger { get; set; }

        [JsonProperty("updated")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Updated { get; set; }

        [JsonProperty("is_app_user")]
        public Boolean IsAppUser { get; set; }

        [JsonProperty("has_2fa")]
        public Boolean Has2FA { get; set; }

        [JsonProperty("locale")]
        public String Locale { get; set; }
    }
}
