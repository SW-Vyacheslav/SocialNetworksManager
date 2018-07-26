using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VKExtension.Responses
{
    public class VkFriendsGetResponse
    {
        [JsonProperty("response")]
        public VkFriendsGetResponseItem Response { get; set; }
    }

    public class VkFriendsGetResponseItem
    {
        [JsonProperty("count")]
        public Int32 Count { get; set; }

        [JsonProperty("items")]
        public Models.VkUser[] Users { get; set; }
    }
}
