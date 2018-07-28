using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters;

namespace VKExtension.Requests
{
    public class VkPhotosGetRequest
    {
        public String OwnerID { get; private set; }
        public Enums.VkPhotoAlbumId AlbumID { get; private set; }
        public String PhotoIDs { get; set; }
        public Boolean Rev { get; set; }
        public Boolean Extended { get; set; }
        public String FeedType { get; set; }
        public String Feed { get; set; }
        public Boolean PhotoSizes { get; set; }
        public String Offset { get; set; }
        public String Count { get; set; }
        public String AccessToken { get; private set; }
        public Enums.VkApiVersion Version { get; private set; }

        public VkPhotosGetRequest(String owner_id, Enums.VkPhotoAlbumId album_id, String access_token, Enums.VkApiVersion version)
        {
            PhotoIDs = null;
            Rev = false;
            Extended = false;
            FeedType = null;
            Feed = null;
            PhotoSizes = false;
            Offset = null;
            Count = null;
            OwnerID = owner_id;
            AlbumID = album_id;
            AccessToken = access_token;
            Version = version;
        }

        public override string ToString()
        {
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["owner_id"] = OwnerID;
            parameters["album_id"] = EnumConverter.ConvertToString<Enums.VkPhotoAlbumId>(AlbumID);
            if (PhotoIDs != null) parameters["photo_ids"] = PhotoIDs;
            parameters["rev"] = Rev == true ? "1" : "0";
            parameters["extended"] = Extended == true ? "1" : "0";
            if (FeedType != null) parameters["feed_type"] = FeedType;
            if (Feed != null) parameters["feed"] = Feed;
            parameters["photo_sizes"] = PhotoSizes == true ? "1" : "0";
            if (Offset != null) parameters["offset"] = Offset;
            if (Count != null) parameters["count"] = Count;
            parameters["access_token"] = AccessToken;
            parameters["v"] = EnumConverter.ConvertToString<Enums.VkApiVersion>(Version);

            return Abilities.VkApiLinkCreator.CreateLink(Enums.VkMethod.GetPhotos,parameters);
        }
    }
}
