using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetworksManager.Contracts;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using mshtml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FacebookExtension
{
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class FacebookExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private String access_token;
        private String user_id;

        private String auth_link;

        private String app_id = "203282280378025";
        private String scope = "user_friends";
        private String redirect_uri = "https://www.facebook.com/connect/login_success.html";

        private WebBrowser webBrowser;
        private WebClient client;

        public bool IsAuthorized { get; private set; }

        public FacebookExtension()
        {
            client = new WebClient();

            auth_link = "https://facebook.com/v3.0/dialog/oauth?response_type=token&display=popup&client_id=" + app_id + "&redirect_uri=" + redirect_uri + "&scope=" + scope;
        }

        #region InfoAboutSocialNetwork
        public string getExtensionName()
        {
            return "Facebook_Extension";
        }

        public string getSocialNetworkName()
        {
            return "Facebook";
        }
        #endregion

        public void Authorization()
        {
            if (IsAuthorized)
            {
                applicationContract.setTextBoxValue("You are authorized in Facebook.");
                return;
            }
            webBrowser = applicationContract.GetWebBrowser();
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            webBrowser.Navigate(auth_link);
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            HTMLDocument document = (HTMLDocument)webBrowser.Document;

            if (document.url.Contains("https://www.facebook.com/connect/login_success.html"))
            {
                webBrowser.IsEnabled = false;
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;

                Regex fieldsPattern = new Regex(@"\w+=\w+");
                MatchCollection matches = fieldsPattern.Matches(document.url);

                for (int i = 0; i < matches.Count; i++)
                {
                    String[] field = matches[i].Value.Split('=');

                    switch(field[0])
                    {
                        case "access_token":
                            access_token = field[1];
                            break;
                        default:
                            break;
                    }
                }

                dynamic json = JObject.Parse(client.DownloadString("https://graph.facebook.com/v3.0/me?fields=id&access_token="+access_token));
                user_id = (String)json.id;
                IsAuthorized = true;
                applicationContract.setTextBoxValue("You are authorized in Facebook.");
            }
        }

        public void GetFriends()
        {
            if (!IsAuthorized) return;

            dynamic json = JObject.Parse(client.DownloadString("https://graph.facebook.com/v3.0/" + user_id + "/friends?access_token=" + access_token));

            if ((int)json.summary.total_count == 0)
            {
                applicationContract.setTextBoxValue("You don't have friends.");
                return;
            }
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < (int)json.summary.total_count; i++)
            {
                builder.AppendFormat("ID:{0}\n" +
                                     "Name:{1}\n\n",
                                     (String)json.data[i].id,
                                     (String)json.data[i].name);
            }

            applicationContract.setTextBoxValue(builder.ToString());
        }
    }
}