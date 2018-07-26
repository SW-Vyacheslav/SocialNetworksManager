using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VKExtension.Models
{
    public class VkUser
    {
        #region Base Fields
        [JsonProperty("id")]
        public Int32 ID { get; set; }

        [JsonProperty("first_name")]
        public String First_Name { get; set; }

        [JsonProperty("last_name")]
        public String Last_Name { get; set; }

        [JsonProperty("deactivated")]
        public String Deactivated { get; set; }
        #endregion

        #region Option Fields
        [JsonProperty("online")]
        public Int32 Online { get; set; }
        #endregion
    }
}
