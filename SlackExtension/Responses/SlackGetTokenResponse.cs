using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Responses
{
    public class SlackGetTokenResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        [JsonProperty("scope")]
        public String Scope { get; set; }

        [JsonProperty("user_id")]
        public String UserID { get; set; }

        [JsonProperty("team_name")]
        public String TeamName { get; set; }

        [JsonProperty("team_id")]
        public String TeamID { get; set; }

        [JsonProperty("error")]
        public String Error { get; set; }
    }
}