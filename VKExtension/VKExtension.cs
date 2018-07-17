using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using SocialNetworksManager.Contracts;
using System.Net;
using System.Windows.Controls;
using mshtml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private String access_token;
        private String user_id;

        private String auth_link;

        private String app_id = "6629531";
        private String scope = "friends";

        private WebClient client;
        private WebBrowser webBrowser;

        public bool IsAuthorized { get; private set; }

        public VKExtension()
        {
            client = new WebClient();

            IsAuthorized = false;

            auth_link = "https://oauth.vk.com/authorize?client_id=" + app_id + "&display=page&redirect_uri=&scope=" + scope + "&response_type=token&v=5.80";
        }

        #region InfoAboutSocialNetwork
        public string getExtensionName()
        {
            return "VK_Extension";
        }

        public string getSocialNetworkName()
        {
            return "VK";
        }
        #endregion

        public void Authorization()
        {
            if (IsAuthorized)
            {
                applicationContract.setTextBoxValue("You are authorized in VK.");
                return;
            }
            webBrowser = applicationContract.GetWebBrowser();
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            webBrowser.Navigate(auth_link);
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            HTMLDocument document = (HTMLDocument)webBrowser.Document;

            if (document.url.Contains("https://oauth.vk.com/blank.html"))
            {
                webBrowser.IsEnabled = false;
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;

                Regex fieldsPattern = new Regex(@"\w+=\w+");
                MatchCollection matches = fieldsPattern.Matches(document.url);
                    
                for (int i = 0; i < matches.Count; i++)
                {
                    String[] field = matches[i].Value.Split('=');

                    switch (field[0])
                    {
                        case "user_id":
                            user_id = field[1];
                            break;
                        case "access_token":
                            access_token = field[1];
                            break;
                        default:
                            break;
                    }
                }
                IsAuthorized = true;
                applicationContract.setTextBoxValue("You are authorized in VK.");
            }
        }

        private String UTF8ToWindows1251(String source)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            return win1251.GetString(win1251Bytes);
        }

        public void GetFriends()
        {
            if (!IsAuthorized) return;
            dynamic json = JObject.Parse(client.DownloadString("https://api.vk.com/method/friends.get?order=name&count=100&fields=all&v=5.80&access_token=" + access_token));
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < (int)json.response.count; i++)
            {
                builder.AppendFormat("ID: {0}\n" +
                                     "First_Name: {1}\n" +
                                     "Last_Name: {2}\n" +
                                     "Status: {3}\n\n",
                                     (String)json.response.items[i].id,
                                     UTF8ToWindows1251((String)json.response.items[i].first_name),
                                     UTF8ToWindows1251((String)json.response.items[i].last_name),
                                     (String)json.response.items[i].online != "0" ? "Online" : "Offline");
            }

            applicationContract.setTextBoxValue(builder.ToString());
        }
    }
}
