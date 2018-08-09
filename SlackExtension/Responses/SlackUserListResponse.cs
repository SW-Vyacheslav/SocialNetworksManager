using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Responses
{
    public class SlackUserListResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("members")]
        public Models.SlackUser[] Members { get; set; }

        [JsonProperty("error")]
        public String Error { get; set; }
    }
}
