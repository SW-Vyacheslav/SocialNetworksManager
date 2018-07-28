using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Newtonsoft.Json;

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
            if (userId == null) return null;

            Enums.VkUserField fields = Enums.VkUserField.ID |
                                       Enums.VkUserField.FirstName |
                                       Enums.VkUserField.LastName |
                                       Enums.VkUserField.Deactivated |
                                       Enums.VkUserField.Online;

            Requests.VkFriendsGetRequest request = new Requests.VkFriendsGetRequest(response.AccessToken,Enums.VkApiVersion.V5_80)
            {
                UserID = Convert.ToString(userId),
                Fields = fields
            };

            String data = Windows1251ToUtf8(webClient.DownloadString(request.ToString()));

            Responses.VkFriendsGetResponse friends = JsonConvert.DeserializeObject<Responses.VkFriendsGetResponse>(data);

            return friends.Response.Users;
        }

        public static Models.VkUser[] Friends_Get(Responses.VkAuthResponse response)
        {
            Enums.VkUserField fields = Enums.VkUserField.ID |
                                       Enums.VkUserField.FirstName |
                                       Enums.VkUserField.LastName |
                                       Enums.VkUserField.Deactivated |
                                       Enums.VkUserField.Online;

            Requests.VkFriendsGetRequest request = new Requests.VkFriendsGetRequest(response.AccessToken,Enums.VkApiVersion.V5_80)
            {
                Fields = fields
            };

            String data = Windows1251ToUtf8(webClient.DownloadString(request.ToString()));

            Responses.VkFriendsGetResponse friends = JsonConvert.DeserializeObject<Responses.VkFriendsGetResponse>(data);

            return friends.Response.Users;
        }

        public static Models.VkPhoto[] Photos_Get(String userId, Responses.VkAuthResponse response)
        {
            if (userId == null) return null;

            Requests.VkPhotosGetRequest request = new Requests.VkPhotosGetRequest(Convert.ToString(userId), Enums.VkPhotoAlbumId.Profile, response.AccessToken, Enums.VkApiVersion.V5_80);

            String data = webClient.DownloadString(request.ToString());

            Responses.VkPhotosGetResponse photos = JsonConvert.DeserializeObject<Responses.VkPhotosGetResponse>(data); ;

            return photos.Response.Photos;
        }

        public static Models.VkPhoto[] Photos_Get(Responses.VkAuthResponse response)
        {
            Requests.VkPhotosGetRequest request = new Requests.VkPhotosGetRequest(response.UserId, Enums.VkPhotoAlbumId.Profile, response.AccessToken, Enums.VkApiVersion.V5_80);

            String data = webClient.DownloadString(request.ToString());

            Responses.VkPhotosGetResponse photos = JsonConvert.DeserializeObject<Responses.VkPhotosGetResponse>(data);

            return photos.Response.Photos;
        }

        public static bool Message_Send(String userId, String message , Responses.VkAuthResponse response)
        {
            if (userId == null) return false;

            Requests.VkMessageSendRequest request = new Requests.VkMessageSendRequest(userId, response.AccessToken, Enums.VkApiVersion.V5_80)
            {
                Message = message
            };

            String data = webClient.DownloadString(request.ToString());

            return true;
        }

        public static bool Message_Send(String message, Responses.VkAuthResponse response)
        {
            Requests.VkMessageSendRequest request = new Requests.VkMessageSendRequest(response.UserId, response.AccessToken, Enums.VkApiVersion.V5_80)
            {
                Message = message
            };

            String data = webClient.DownloadString(request.ToString());

            return true;
        }
    }
}
