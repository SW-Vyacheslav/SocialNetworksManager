using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Responses
{
    public class SlackIMListResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("ims")]
        public Models.SlackIM[] IMs { get; set; }

        [JsonProperty("error")]
        public String Error { get; set; }
    }
}
