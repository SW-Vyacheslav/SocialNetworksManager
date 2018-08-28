using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Responses
{
    public class SlackUsersInfoResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("user")]
        public Models.SlackUser User { get; set; }

        [JsonProperty("error")]
        public String Error { get; set; }
    }
}
