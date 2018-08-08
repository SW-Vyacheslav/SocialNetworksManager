using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

using SocialNetworksManager.Contracts;
using SocialNetworksManager.DataPresentation;

using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Utils;
using VkNet.Exception;

namespace VkExtension
{
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VkExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract), AllowRecomposition = true)]
        public IApplicationContract applicationContract;

        private VkApi vk_api;
        
        public VkExtension()
        {
            vk_api = new VkApi();
        }

        public String getSocialNetworkName()
        {
            return "VK";
        }

        public String getExtensionName()
        {
            return "VK_Extension";
        }

        public bool GetAuthStatus()
        {
            return vk_api.IsAuthorized;
        }

        public void Authorization()
        {
            if (vk_api.IsAuthorized) return;

            AuthControl authControl = new AuthControl(applicationContract);
            applicationContract.OpenSpecialWindow(authControl);

            ApiAuthParams authParams = new ApiAuthParams();
            authParams.ApplicationId = Properties.AppSettings.Default.client_id;
            authParams.Login = authControl.GetLogin();
            authParams.Password = authControl.GetPassword();
            authParams.Settings = Settings.All;

            try
            {
                vk_api.Authorize(authParams);
            }
            catch (VkApiException ex)
            {
                
            }
        }

        public void GetFriends()
        {
            if (!vk_api.IsAuthorized) return;

            FriendsGetParams getParams = new FriendsGetParams();
            getParams.Fields = ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Online;

            VkCollection<User> friends = null;

            try
            {
                friends = vk_api.Friends.Get(getParams, true);
            }
            catch (VkApiException ex)
            {
                
            }

            List<FriendsListItem> friendsItems = new List<FriendsListItem>();

            foreach (User friend in friends)
            {
                FriendsListItem friendItem = new FriendsListItem();
                friendItem.SocialNetworkName = getSocialNetworkName();
                friendItem.FriendName = String.Format("{0} {1}",friend.FirstName,friend.LastName);
                friendItem.Status = friend.Online == true ? "Online" : "Offline";
                friendsItems.Add(friendItem);
            }

            applicationContract.AddItemsToFriendsList(friendsItems);
        }

        public void GetPhotos()
        {
            if (!vk_api.IsAuthorized) return;

            PhotoGetParams getParams = new PhotoGetParams();
            getParams.AlbumId = PhotoAlbumType.Profile;
            getParams.PhotoSizes = true;

            VkCollection<Photo> photos = null;

            try
            {
                photos = vk_api.Photo.Get(getParams, true);
            }
            catch(VkApiException ex)
            {

            }

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            foreach (Photo photo in photos)
            {
                PhotosListItem photoItem = new PhotosListItem();
                photoItem.SocialNetworkName = getSocialNetworkName();
                photoItem.PhotoSource = photo.Sizes[2].Url.ToString();

                photosItems.Add(photoItem);
            }

            applicationContract.AddItemsToPhotosList(photosItems);
        }

        public void SendMessage()
        {
            if (!vk_api.IsAuthorized) return;

            MessagesSendParams sendParams = new MessagesSendParams();
            sendParams.UserId = vk_api.UserId;
            sendParams.Message = "test";

            try
            {
                vk_api.Messages.Send(sendParams);
            }
            catch (VkApiException ex)
            {

            }
        }
    }
}
