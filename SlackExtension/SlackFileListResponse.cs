using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SlackAPI;

namespace SlackExtension
{
    public class SlackFileListResponse
    {
        [JsonProperty("ok")]
        public Boolean Ok { get; set; }

        [JsonProperty("files")]
        public SlackFile[] Files { get; set; }
    }

    public class SlackFile
    {
        public String thumb_80 { get; set; }
        public String mimetype { get; set; }
    }
}
