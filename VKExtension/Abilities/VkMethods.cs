using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Newtonsoft.Json;

using Converters;

namespace VKExtension.Abilities
{
    public static class VkMethods
    {
        private static WebClient webClient = new WebClient();

        private static String Windows1251ToUtf8(String text)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(text);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            return win1251.GetString(win1251Bytes);
        }

        public static Models.VkUser[] Friends_Get(String userId,Responses.VkAuthResponse response)
        {
            Enums.VkUserField fields = Enums.VkUserField.ID | 
                                       Enums.VkUserField.FirstName | 
                                       Enums.VkUserField.LastName |
                                       Enums.VkUserField.Deactivated |
                                       Enums.VkUserField.Online;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            if (userId != null) parameters["user_id"] = userId;
            parameters["fields"] = EnumConverter.ConvertToString<Enums.VkUserField>(fields);
            parameters["order"] = "hints";
            parameters["access_token"] = response.AccessToken;
            parameters["v"] = EnumConverter.ConvertToString<Enums.VkApiVersion>(Enums.VkApiVersion.V5_80);

            String request = VkApiLinkCreator.CreateLink(Enums.VkMethod.GetFriends, parameters);
            String data = Windows1251ToUtf8(webClient.DownloadString(request));

            Responses.VkFriendsGetResponse friends = JsonConvert.DeserializeObject<Responses.VkFriendsGetResponse>(data);

            return friends.Response.Users;
        }

        public static Models.VkPhoto[] Photos_Get(String userId, Responses.VkAuthResponse response)
        {
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            if (userId != null) parameters["owner_id"] = userId;
            parameters["album_id"] = "profile";
            parameters["access_token"] = response.AccessToken;
            parameters["v"] = EnumConverter.ConvertToString<Enums.VkApiVersion>(Enums.VkApiVersion.V5_80);

            String request = VkApiLinkCreator.CreateLink(Enums.VkMethod.GetPhotos,parameters);
            String data = webClient.DownloadString(request);

            Responses.VkPhotosGetResponse photos = JsonConvert.DeserializeObject<Responses.VkPhotosGetResponse>(data); ;

            return photos.Response.Photos;
        }
    }
}
