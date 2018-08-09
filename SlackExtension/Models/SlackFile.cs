using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Models
{
    public class SlackFile
    {
        [JsonProperty("thumb_80")]
        public String Thumb80 { get; set; }

        [JsonProperty("mimetype")]
        public String MimeType { get; set; }
    }
}
