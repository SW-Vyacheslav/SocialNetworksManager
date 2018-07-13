using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using SocialNetworksManager.Contracts;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract),AllowRecomposition = true)]
        private IApplicationContract application;

        private String access_token = null;

        private String auth_link;
        private String getfriends_link;

        private String app_id = "6629531";
        private String scope = "friends";

        private WebClient client;
        private bool isAuthorized = false;

        public VKExtension()
        {
            client = new WebClient();

            auth_link = "https://oauth.vk.com/authorize?client_id="+app_id+"&display=page&redirect_uri=&scope="+scope+"&response_type=token&v=5.80";
            getfriends_link = "https://api.vk.com/methods/friends.get?order=hints&count=100&access_token="+access_token+"&v = 5.80";
        }

        public void Authorization()
        {
            if(isAuthorized) return;
            access_token = application.openAuthWindow(auth_link);
            isAuthorized = true;
        }

        [AvailableInUI(UIName = "Friends")]
        public void getFriends()
        {
            application.setTextBoxValue(client.DownloadString(getfriends_link));
        }

        public string getExtensionName()
        {
            return "VK Extension";
        }

        public string getSocialNetworkName()
        {
            return "VK";
        }
    }
}
