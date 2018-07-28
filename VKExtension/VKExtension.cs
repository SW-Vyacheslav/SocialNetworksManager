using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using SocialNetworksManager.Contracts;

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

            Enums.VkAuthAppAccessRule rules = Enums.VkAuthAppAccessRule.Friends |
                                              Enums.VkAuthAppAccessRule.Photos | 
                                              Enums.VkAuthAppAccessRule.Messages;

            Requests.VkAuthRequest request = new Requests.VkAuthRequest("6629531",rules,Enums.VkApiVersion.V5_80)
            {
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

            Models.VkUser[] friends = Abilities.VkMethods.Friends_Get(response);

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

            Models.VkPhoto[] photos = Abilities.VkMethods.Photos_Get(response);

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

            Models.VkUser user = (Models.VkUser)applicationContract.getSelectedItem();

            String userID = Convert.ToString(user.ID);

            Abilities.VkMethods.Message_Send(userID, applicationContract.getMessageText(),response);
        }
    }

    public class PhotosListItem
    {
        public BitmapImage img { get; set; }
    }
}
