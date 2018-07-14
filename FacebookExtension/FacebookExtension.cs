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

namespace FacebookExtension
{
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class FacebookExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private String access_token;

        private String auth_link;

        private String app_id = "203282280378025";
        private String scope = "user_friends";
        private String redirect_uri = "https://www.facebook.com/connect/login_success.html";

        private WebBrowser webBrowser;

        private bool isAuthorized = false;

        public FacebookExtension()
        {
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
            if (isAuthorized)
            {
                applicationContract.setTextBoxValue("You are authorized in Facebook.");
                return;
            }
            webBrowser = applicationContract.GetWebBrowser();
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            webBrowser.Visibility = System.Windows.Visibility.Visible;
            webBrowser.Navigate(auth_link);
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            HTMLDocument document = (HTMLDocument)webBrowser.Document;

            if (document.url.Contains("access_token"))
            {
                webBrowser.Visibility = System.Windows.Visibility.Hidden;
                access_token = document.url.Substring(document.url.IndexOf('=') + 1, document.url.IndexOf('&') - (document.url.IndexOf('=') + 1));
                applicationContract.setTextBoxValue(String.Format("Access_token:{0}\nURL:{1}", access_token, document.url));
                isAuthorized = true;
            }
        }
    }
}
