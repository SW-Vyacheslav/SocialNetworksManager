using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace VKExtension.Responses
{
    public class VkPhotosGetResponse
    {
        [JsonProperty("response")]
        public VkPhotosGetResponseItem Response { get; set; }
    }

    public class VkPhotosGetResponseItem
    {
        [JsonProperty("count")]
        public Int32 Count { get; set; }

        [JsonProperty("items")]
        public Models.VkPhoto[] Photos { get; set; }
    }
}
