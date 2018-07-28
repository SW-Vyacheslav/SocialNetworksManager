using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters;

namespace VKExtension.Requests
{
    public class VkFriendsGetRequest
    {
        public String UserID { get; set; }
        public String Order { get; set; }
        public String ListID { get; set; }
        public String Count { get; set; }
        public String Offset { get; set; }
        public Enums.VkUserField Fields { get; set; }
        public String NameCase { get; set; }
        public Enums.VkApiVersion Version { get; private set; }
        public String AccessToken { get; private set; }

        public VkFriendsGetRequest(String access_token,Enums.VkApiVersion version)
        {
            UserID = null;
            Order = null;
            ListID = null;
            Count = null;
            Offset = null;
            NameCase = null;
            AccessToken = access_token;
            Version = version;
        }

        public override string ToString()
        {
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            if (UserID != null) parameters["user_id"] = UserID;
            if (Order != null) parameters["order"] = Order;
            if (ListID != null) parameters["list_id"] = ListID;
            if (Count != null) parameters["count"] = Count;
            if (Offset != null) parameters["offset"] = Offset;
            parameters["fields"] = EnumConverter.ConvertToString<Enums.VkUserField>(Fields);
            if (NameCase != null) parameters["name_case"] = NameCase;
            parameters["access_token"] = AccessToken;
            parameters["v"] = EnumConverter.ConvertToString<Enums.VkApiVersion>(Version);

            return Abilities.VkApiLinkCreator.CreateLink(Enums.VkMethod.GetFriends,parameters);
        }
    }
}
