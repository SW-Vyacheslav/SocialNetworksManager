using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FacebookExtension.Abilities
{
    public static class FacebookMethods
    {
        private static WebClient webClient = new WebClient();

        public static String getTokenUserID(Responses.FacebookAuthResponse response)
        {
            String ID = null;

            String data = webClient.DownloadString(String.Format("https://graph.facebook.com/v3.0/me?fields=id&access_token={0}",response.AccessToken));

            dynamic json = JObject.Parse(data);

            ID = (String)json.id;

            return ID;
        }

        public static Models.User[] getFriends(String userID, Responses.FacebookAuthResponse response)
        {
            Models.User[] users = null;

            String data = webClient.DownloadString(String.Format("https://graph.facebook.com/v3.0/{0}/friends?fields=id,first_name,last_name&access_token={1}", userID,response.AccessToken));

            dynamic json = JObject.Parse(data);

            JArray array = json.data;

            if (array.Count == 0) return users;

            users = new Models.User[(int)json.summary.total_count];

            for (int i = 0; i < (int)json.summary.total_count; i++)
            {
                Models.User user = new Models.User();
                user.ID = (String)json.data[i].id;
                user.First_Name = (String)json.first_name;
                user.Last_Name = (String)json.last_name;
                users[i] = user;
            }

            return users;
        }
    }
}
