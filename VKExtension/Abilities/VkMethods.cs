using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static Models.User[] Friends_Get(String user_id,Responses.VkAuthResponse response)
        {
            Models.User[] users = null;

            String request = null;

            if (user_id != null)
            {
                request = String.Format("https://api.vk.com/method/friends.get?user_id={0}&fields=id,first_name,last_name,online&order=hints&access_token={1}&v=5.80", user_id,response.AccessToken);
            }
            else
            {
                request = String.Format("https://api.vk.com/method/friends.get?&fields=id,first_name,last_name,online&order=hints&access_token={0}&v=5.80",response.AccessToken);
            }

            String data = webClient.DownloadString(request);

            dynamic json = JObject.Parse(data);

            users = new Models.User[(int)json.response.count];

            for (int i = 0; i < (int)json.response.count; i++)
            {
                users[i] = new Models.User();
                users[i].ID = Convert.ToString((int)json.response.items[i].id);
                users[i].First_Name = Windows1251ToUtf8((String)json.response.items[i].first_name);
                users[i].Last_Name = Windows1251ToUtf8((String)json.response.items[i].last_name);
                users[i].Online = (String)json.response.items[i].online == "1" ? true : false;
            }

            return users;
        }
    }
}
