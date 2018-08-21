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

            if (authControl.IsCanceled) return;

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
            getParams.Order = FriendsOrder.Name;

            VkCollection<User> friends = null;

            try
            {
                friends = vk_api.Friends.Get(getParams, true);
            }
            catch (VkApiException ex)
            {
                return;
            }

            List<FriendsListItem> friendsItems = new List<FriendsListItem>();

            foreach (User friend in friends)
            {
                FriendsListItem friendItem = new FriendsListItem();
                friendItem.SocialNetworkName = getSocialNetworkName();
                friendItem.FriendName = String.Format("{0} {1}",friend.FirstName,friend.LastName);
                friendItem.ID = Convert.ToString(friend.Id);
                friendsItems.Add(friendItem);
            }

            applicationContract.AddItemsToFriendsList(friendsItems);
        }

        public void GetPhotos()
        {
            if (!vk_api.IsAuthorized) return;

            PhotoGetParams getParamsProfile = new PhotoGetParams();
            getParamsProfile.AlbumId = PhotoAlbumType.Profile;
            getParamsProfile.PhotoSizes = true;

            PhotoGetParams getParamsWall = new PhotoGetParams();
            getParamsWall.AlbumId = PhotoAlbumType.Wall;
            getParamsWall.PhotoSizes = true;

            PhotoGetParams getParamsSaved = new PhotoGetParams();
            getParamsSaved.AlbumId = PhotoAlbumType.Saved;
            getParamsSaved.PhotoSizes = true;

            VkCollection<Photo> photosProfile = null;
            VkCollection<Photo> photosWall = null;
            VkCollection<Photo> photosSaved = null;

            try
            {
                photosProfile = vk_api.Photo.Get(getParamsProfile, true);
                photosWall = vk_api.Photo.Get(getParamsWall, true);
                photosSaved = vk_api.Photo.Get(getParamsSaved, true);
            }
            catch(VkApiException ex)
            {
                return;
            }

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            foreach (Photo photo in photosProfile)
            {
                PhotosListItem photoItem = new PhotosListItem();
                photoItem.SocialNetworkName = getSocialNetworkName();
                photoItem.PhotoSource = photo.Sizes[photo.Sizes.Count-1].Url.ToString();

                photosItems.Add(photoItem);
            }

            foreach (Photo photo in photosWall)
            {
                PhotosListItem photoItem = new PhotosListItem();
                photoItem.SocialNetworkName = getSocialNetworkName();
                photoItem.PhotoSource = photo.Sizes[photo.Sizes.Count - 1].Url.ToString();

                photosItems.Add(photoItem);
            }

            foreach (Photo photo in photosSaved)
            {
                PhotosListItem photoItem = new PhotosListItem();
                photoItem.SocialNetworkName = getSocialNetworkName();
                photoItem.PhotoSource = photo.Sizes[photo.Sizes.Count - 1].Url.ToString();

                photosItems.Add(photoItem);
            }

            applicationContract.AddItemsToPhotosList(photosItems);
        }

        public void SendMessageToSelectedFriends()
        {
            if (!vk_api.IsAuthorized) return;

            List<FriendsListItem> items = applicationContract.GetFriendsListItems();
            List<SendMessageStatus> statuses = new List<SendMessageStatus>();

            MessagesSendParams sendParams = new MessagesSendParams();

            foreach (FriendsListItem item in items)
            {
                if (!item.SocialNetworkName.Equals(getSocialNetworkName())) continue;
                if (!item.IsChecked) continue;

                sendParams.UserId = Convert.ToInt64(item.ID);
                sendParams.Message = applicationContract.GetMessage();

                SendMessageStatus status = new SendMessageStatus();
                status.SocialNetworkName = getSocialNetworkName();
                status.UserName = item.FriendName;
                status.IsMessageSended = true;

                try
                {
                    vk_api.Messages.Send(sendParams);
                }
                catch (VkApiException ex)
                {
                    status.IsMessageSended = false;
                }

                statuses.Add(status);
            }
            applicationContract.AddSendMessageStatuses(statuses);
        }
    }
}
