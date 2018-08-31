using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackExtension.Models
{
    public class SlackFile
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("created")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Created { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("mimetype")]
        public String MimeType { get; set; }

        [JsonProperty("filetype")]
        public String FileType { get; set; }

        [JsonProperty("pretty_type")]
        public String PrettyType { get; set; }

        [JsonProperty("user")]
        public String User { get; set; }

        [JsonProperty("editable")]
        public Boolean Editable { get; set; }

        [JsonProperty("size")]
        public UInt64 Size { get; set; }

        [JsonProperty("mode")]
        public String Mode { get; set; }

        [JsonProperty("is_external")]
        public Boolean IsExternal { get; set; }

        [JsonProperty("external_type")]
        public String ExternalType { get; set; }

        [JsonProperty("is_public")]
        public Boolean IsPublic { get; set; }

        [JsonProperty("public_url_shared")]
        public Boolean PublicURLShared { get; set; }

        [JsonProperty("display_as_bot")]
        public Boolean DisplayAsBot { get; set; }

        [JsonProperty("username")]
        public String UserName { get; set; }

        [JsonProperty("url_private")]
        public String URLPrivate { get; set; }

        [JsonProperty("url_private_download")]
        public String URLPrivateDownload { get; set; }

        [JsonProperty("thumb_64")]
        public String Thumb64 { get; set; }

        [JsonProperty("thumb_80")]
        public String Thumb80 { get; set; }

        [JsonProperty("thumb_360")]
        public String Thumb360 { get; set; }

        [JsonProperty("thumb_360_w")]
        public UInt64 Thumb360Width { get; set; }

        [JsonProperty("thumb_360_h")]
        public UInt64 Thumb360Height { get; set; }

        [JsonProperty("thumb_480")]
        public String Thumb480 { get; set; }

        [JsonProperty("thumb_480_w")]
        public UInt64 Thumb480Width { get; set; }

        [JsonProperty("thumb_480_h")]
        public UInt64 Thumb480Height { get; set; }

        [JsonProperty("thumb_160")]
        public String Thumb160 { get; set; }

        [JsonProperty("thumb_720")]
        public String Thumb720 { get; set; }

        [JsonProperty("thumb_720_w")]
        public UInt64 Thumb720Width { get; set; }

        [JsonProperty("thumb_720_h")]
        public UInt64 Thumb720Height { get; set; }

        [JsonProperty("image_exif_rotation")]
        public Int64 ImageExifRoation { get; set; }

        [JsonProperty("original_w")]
        public UInt64 OriginalWidth { get; set; }

        [JsonProperty("original_h")]
        public UInt64 OriginalHeight { get; set; }

        [JsonProperty("permalink")]
        public String Permalink { get; set; }

        [JsonProperty("permalink_public")]
        public String PermalinkPublic { get; set; }

        [JsonProperty("channels")]
        public String[] Channels { get; set; }

        [JsonProperty("groups")]
        public String[] Groups { get; set; }

        [JsonProperty("ims")]
        public String[] IMs { get; set; }

        [JsonProperty("comments_count")]
        public UInt64 CommentsCount { get; set; }
    }
}