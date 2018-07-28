using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters;

namespace VKExtension.Requests
{
    public class VkMessageSendRequest
    {
        public String UserID { get; private set; }
        public String Message { get; set; }
        public String AccessToken { get; private set; }
        public Enums.VkApiVersion Version { get; private set; }

        public VkMessageSendRequest(String user_id, String access_token, Enums.VkApiVersion version)
        {
            UserID = user_id;
            Message = "";
            AccessToken = access_token;
            Version = version;
        }

        public override string ToString()
        {
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["user_id"] = UserID;
            parameters["message"] = Message;
            parameters["access_token"] = AccessToken;
            parameters["v"] = EnumConverter.ConvertToString<Enums.VkApiVersion>(Version);

            return Abilities.VkApiLinkCreator.CreateLink(Enums.VkMethod.SendMessage,parameters);
        }
    }
}
