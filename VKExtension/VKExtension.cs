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

        private List<VkApi> users_api;
        private List<AccountSaveProfileInfoParams> users_accounts;
        private Int64 users_count;
        
        public VkExtension()
        {
            users_api = new List<VkApi>();
            users_accounts = new List<AccountSaveProfileInfoParams>();
            users_count = 0;
        }

        public String getSocialNetworkName()
        {
            return "VK";
        }

        public String getExtensionName()
        {
            return "VK_Extension";
        }

        public List<UserInfo> getAuthorizedUsers()
        {
            List<UserInfo> users = new List<UserInfo>();

            for (int i = 0;i < users_count; i++)
            {
                if (!users_api[i].IsAuthorized) continue;

                UserInfo userInfo = new UserInfo();
                userInfo.Name = getUserAccountName(i);
                userInfo.ID = Convert.ToString(users_api[i].UserId);
                userInfo.SocialNetworkName = getSocialNetworkName();
                users.Add(userInfo);
            }

            return users;
        }

        private String getUserAccountName(int index)
        {
            return String.Format("{0} {1}", users_accounts[index].FirstName, users_accounts[index].LastName);
        }

        public void Authorization()
        {
            AuthControl authControl = new AuthControl(applicationContract);
            applicationContract.OpenSpecialWindow(authControl);

            if (authControl.IsCanceled) return;

            VkApi vk_api = new VkApi();

            ApiAuthParams authParams = new ApiAuthParams();
            authParams.ApplicationId = Convert.ToUInt64(Properties.Resources.client_id);
            authParams.Login = authControl.GetLogin();
            authParams.Password = authControl.GetPassword();
            authParams.Settings = Settings.Friends | Settings.Photos | Settings.Wall | Settings.Messages;

            try
            {
                vk_api.Authorize(authParams);
            }
            catch (VkApiException ex)
            {
                applicationContract.OpenSpecialWindow("Vk Auth Error.");
                return;
            }

            for (int i = 0; i < users_count; i++)
            {
                if (users_api[i].UserId.Equals(vk_api.UserId))
                {
                    applicationContract.OpenSpecialWindow("You already authorized.");
                    return;
                }
            }

            users_api.Add(vk_api);
            users_accounts.Add(vk_api.Account.GetProfileInfo());
            users_count++;
        }

        public void GetFriends()
        {
            for (int i = 0;i < users_count; i++)
            {
                if (!users_api[i].IsAuthorized) continue;

                FriendsGetParams getParams = new FriendsGetParams();
                getParams.Fields = ProfileFields.FirstName | ProfileFields.LastName;
                getParams.Order = FriendsOrder.Name;

                VkCollection<User> friends = null;

                try
                {
                    friends = users_api[i].Friends.Get(getParams, true);
                }
                catch (VkApiException ex)
                {
                    continue;
                }

                List<FriendsListItem> friendsItems = new List<FriendsListItem>();

                foreach (User friend in friends)
                {
                    FriendsListItem friendItem = new FriendsListItem();
                    friendItem.User = new UserInfo();
                    friendItem.Friend = new UserInfo();
                    friendItem.SocialNetworkName = getSocialNetworkName();
                    friendItem.Friend.Name = String.Format("{0} {1}", friend.FirstName, friend.LastName);
                    friendItem.Friend.ID = Convert.ToString(friend.Id);
                    friendItem.User.Name = getUserAccountName(i);
                    friendItem.User.ID = Convert.ToString(users_api[i].UserId);
                    friendsItems.Add(friendItem);
                }

                applicationContract.AddItemsToFriendsList(friendsItems);
            }
        }

        public void GetPhotos(string user_id)
        {
            VkCollection<Photo> photos = null;

            Boolean isFriends = false;

            for (int i = 0; i < users_api.Count; i++)
            {
                if (users_api[i].Friends.AreFriends(new List<long>() { Convert.ToInt64(user_id) })[0].FriendStatus == VkNet.Enums.FriendStatus.Friend)
                {
                    PhotoGetAllParams getAllParams = new PhotoGetAllParams();
                    getAllParams.OwnerId = Convert.ToInt64(user_id);
                    getAllParams.PhotoSizes = true;

                    try
                    {
                        photos = users_api[i].Photo.GetAll(getAllParams);
                    }
                    catch (VkApiException ex)
                    {
                        return;
                    }

                    isFriends = true;

                    break;
                }
            }

            if (!isFriends)
            {
                for (int i = 0; i < users_api.Count; i++)
                {
                    if (users_api[i].UserId == Convert.ToInt64(user_id))
                    {
                        PhotoGetAllParams getAllParams = new PhotoGetAllParams();
                        getAllParams.PhotoSizes = true;

                        try
                        {
                            photos = users_api[i].Photo.GetAll(getAllParams);
                        }
                        catch (VkApiException ex)
                        {
                            return;
                        }

                        break;
                    }
                }
            }

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            foreach (Photo photo in photos)
            {
                PhotosListItem photoItem = new PhotosListItem(photo.Sizes[photo.Sizes.Count - 1].Url);

                photosItems.Add(photoItem);
            }

            applicationContract.AddItemsToPhotosList(photosItems);
        }

        public void SendMessageToSelectedFriends()
        {
            for (int i = 0; i < users_count; i++)
            {
                if (!users_api[i].IsAuthorized) continue;

                List<FriendsListItem> items = applicationContract.GetFriendsListItems();
                List<SendMessageStatus> statuses = new List<SendMessageStatus>();

                MessagesSendParams sendParams = new MessagesSendParams();

                foreach (FriendsListItem item in items)
                {
                    if (!item.SocialNetworkName.Equals(getSocialNetworkName())) continue;
                    if (!item.IsChecked) continue;
                    if (item.User.ID != Convert.ToString(users_api[i].UserId)) continue;

                    sendParams.UserId = Convert.ToInt64(item.Friend.ID);
                    sendParams.Message = applicationContract.GetMessage();

                    SendMessageStatus status = new SendMessageStatus();
                    status.SocialNetworkName = getSocialNetworkName();
                    status.UserNameTo = item.Friend.Name;
                    status.UserNameFrom = getUserAccountName(i);
                    status.IsMessageSended = true;

                    try
                    {
                        users_api[i].Messages.Send(sendParams);
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
}
