﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using SocialNetworksManager.Contracts;
using System.Net;
using System.Windows.Controls;
using mshtml;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private String access_token;

        private String auth_link;

        private String app_id = "6629531";
        private String scope = "friends";

        private WebClient client;
        private WebBrowser webBrowser;

        private bool isAuthorized = false;

        public VKExtension()
        {
            client = new WebClient();

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
            if (isAuthorized)
            {
                applicationContract.setTextBoxValue("You are authorized in VK.");
                return;
            }
            webBrowser = applicationContract.GetWebBrowser();
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            webBrowser.Visibility = System.Windows.Visibility.Visible;
            webBrowser.Navigate(auth_link);
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
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
