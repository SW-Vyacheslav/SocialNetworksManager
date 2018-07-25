using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows.Controls;
using System.Text.RegularExpressions;

using SocialNetworksManager.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using VKExtension.Requests;
using VKExtension.Responses;
using VKExtension.Models;
using VKExtension.Abilities;

using mshtml;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private VkAuthResponse response;

        public bool IsAuthorized { get; private set; }

        public VKExtension()
        {
            IsAuthorized = false;
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
                applicationContract.setInfoValue("You are authorized in VK.");
                return;
            }
            VkAuthRequest request = new VkAuthRequest()
            {
                AppId = "6629531",
                Version = "5.80",
                Scope = Enums.VkAuthAppAccessRule.Friends,
                Revoke = "1"
            };

            VkAuthForm authForm = new VkAuthForm(request);
            response = authForm.Authorize();
            if (response == null)
            {
                applicationContract.setInfoValue("VkAuth ERROR!");
            }
            else if (!response.Error)
            {
                IsAuthorized = true;
                applicationContract.setInfoValue("VkAuth OK!");
            }
            else applicationContract.setInfoValue("VkAuth ERROR!");
        }

        public void GetFriends()
        {
            if (!IsAuthorized) return;

            User[] friends = VkMethods.Friends_Get(null,response);

            if(friends == null)
            {
                applicationContract.setInfoValue("Friends list is empty.");
                return;
            }

            applicationContract.setFriendsListItemsSource(friends.ToList());
        }
    }
}
