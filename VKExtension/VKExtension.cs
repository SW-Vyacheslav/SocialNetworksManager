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

using mshtml;
using System.Windows.Media.Imaging;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        private IApplicationContract applicationContract;

        private Responses.VkAuthResponse response;

        public bool IsAuthorized { get; private set; }

        public VKExtension()
        {
            IsAuthorized = false;
        }

        public string getExtensionName()
        {
            return "VK_Extension";
        }

        public string getSocialNetworkName()
        {
            return "VK";
        }

        public void Authorization()
        {
            if (IsAuthorized)
            {
                applicationContract.setInfoValue("You are authorized in VK.");
                return;
            }
            Requests.VkAuthRequest request = new Requests.VkAuthRequest()
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

            Models.VkUser[] friends = Abilities.VkMethods.Friends_Get(null,response);

            if(friends == null)
            {
                applicationContract.setInfoValue("Friends list is empty.");
                return;
            }

            applicationContract.setFriendsListItemsSource(friends.ToList());
        }

        public void GetPhotos()
        {
            if (!IsAuthorized) return;

            Models.VkPhoto[] photos = Abilities.VkMethods.Photos_Get(null,response);

            if (photos == null)
            {
                applicationContract.setInfoValue("Friends list is empty.");
                return;
            }

            List<PhotosListItem> items = new List<PhotosListItem>();

            foreach (Models.VkPhoto photo in photos)
            {
                PhotosListItem item = new PhotosListItem();
                BitmapImage img = new BitmapImage(new Uri(photo.Sizes[0].Source));
                item.img = img;
                items.Add(item);
            }

            applicationContract.setPhotosListItemsSource(items);
        }

        public void GetNewsFeed()
        {
            if (!IsAuthorized) return;
        }

        public void SendMessage()
        {
            if (!IsAuthorized) return;
        }
    }

    public class PhotosListItem
    {
        public BitmapImage img { get; set; }
    }
}
