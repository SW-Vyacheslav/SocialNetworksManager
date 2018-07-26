using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VKExtension.Models
{
    public class VkPhoto
    {
        [JsonProperty("id")]
        public Int32 ID { get; set; }

        [JsonProperty("album_id")]
        public Int32 AlbumID { get; set; }

        [JsonProperty("owner_id")]
        public Int32 OwnerID { get; set; }

        [JsonProperty("user_id")]
        public Int32 UserID { get; set; }

        [JsonProperty("text")]
        public String Text { get; set; }

        [JsonProperty("date")]
        public Int32 Date { get; set; }

        [JsonProperty("sizes")]
        public VkPhotoDescribeFormat[] Sizes { get; set; }

        [JsonProperty("width")]
        public Int32 Width { get; set; }

        [JsonProperty("height")]
        public Int32 Height { get; set; }
    }

    public class VkPhotoDescribeFormat
    {
        [JsonProperty("url")]
        public String Source { get; set; }

        [JsonProperty("width")]
        public Int32 Width { get; set; }

        [JsonProperty("height")]
        public Int32 Height { get; set; }

        [JsonProperty("type")]
        public String Type { get; set; }
    }
}
