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

        private Responses.FacebookAuthResponse response;

        public  bool IsAuthorized { get; private set; }

        public FacebookExtension()
        {
            IsAuthorized = false;
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
                applicationContract.setInfoValue("You are authorized in Facebook.");
                return;
            }
            Requests.FacebookAuthRequest request = new Requests.FacebookAuthRequest()
            {
                AppId = "203282280378025",
                Scope = Enums.FacebookAuthAppPermission.Friends | Enums.FacebookAuthAppPermission.Photos,
                Version = "3.0"
            };
            FacebookAuthForm authForm = new FacebookAuthForm(request);
            response = authForm.Authorize();
            if (response == null)
            {
                applicationContract.setInfoValue("FacebookAuth ERROR!");
            }
            else if(!response.Error)
            {
                IsAuthorized = true;
                applicationContract.setInfoValue("FacebookAuth OK!");
            }  
            else applicationContract.setInfoValue("FacebookAuth ERROR!");
        }

        public void GetFriends()
        {
            if (!IsAuthorized) return;

            String userID = Abilities.FacebookMethods.getTokenUserID(response);
            Models.User[] users = Abilities.FacebookMethods.getFriends(userID, response);

            if (users == null)
            {
                applicationContract.setInfoValue("Friends list is empty");
                return;
            }
            
            applicationContract.setFriendsListItemsSource(users.ToList());
        }
    }
}