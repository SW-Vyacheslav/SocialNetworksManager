using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SlackExtension.Models
{
    public class SlackIM
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("is_im")]
        public Boolean IsIM { get; set; }

        [JsonProperty("user")]
        public String User { get; set; }

        [JsonProperty("created")]
        public Int32 Created { get; set; }

        [JsonProperty("is_user_deleted")]
        public Boolean IsUserDeleted { get; set; }
    }
}
