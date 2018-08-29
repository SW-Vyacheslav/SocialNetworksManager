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

        //For many users
        private List<VkApi> users_api;
        private List<AccountSaveProfileInfoParams> users_accounts;
        private Int64 users_count;

        //For photos loading
        private String current_photos_user_id = "";
        private Int64 photos_show_count = 20;
        private Int64 current_photos_offset = 0;
        private List<Photo> current_user_photos = null;
        private PhotoGetParams wallPhotosGetParams = null;
        private PhotoGetParams profilePhotosGetParams = null;
        private PhotoGetParams savedPhotosGetParams = null;
        private PhotoGetAlbumsParams albumsGetParams = null;

        public VkExtension()
        {
            users_api = new List<VkApi>();
            users_accounts = new List<AccountSaveProfileInfoParams>();
            users_count = 0;

            current_user_photos = new List<Photo>();

            wallPhotosGetParams = new PhotoGetParams();
            profilePhotosGetParams = new PhotoGetParams();
            savedPhotosGetParams = new PhotoGetParams();
            wallPhotosGetParams.AlbumId = PhotoAlbumType.Wall;
            profilePhotosGetParams.AlbumId = PhotoAlbumType.Profile;
            savedPhotosGetParams.AlbumId = PhotoAlbumType.Saved;
            wallPhotosGetParams.PhotoSizes = true;
            profilePhotosGetParams.PhotoSizes = true;
            savedPhotosGetParams.PhotoSizes = true;

            albumsGetParams = new PhotoGetAlbumsParams();
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
            authParams.Settings = Settings.Friends | Settings.Photos | Settings.Wall | Settings.Messages | Settings.Groups;

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

        public void RefreshPhotos(String user_id)
        {
            applicationContract.ClearItemsFromPhotosList();
            current_photos_user_id = "";
            current_photos_offset = 0;
            GetPhotos(user_id);
        }

        public void GetPhotos(String user_id)
        {
            if(user_id != current_photos_user_id)
            {
                long owner_id = Convert.ToInt64(user_id);
                wallPhotosGetParams.OwnerId = owner_id;
                profilePhotosGetParams.OwnerId = owner_id;
                savedPhotosGetParams.OwnerId = owner_id;

                current_photos_user_id = user_id;
                current_photos_offset = 0;

                current_user_photos.Clear();
                applicationContract.ClearItemsFromPhotosList();

                albumsGetParams.OwnerId = owner_id;
                List<PhotoAlbum> user_albums = new List<PhotoAlbum>();

                for (int i = 0; i < users_api.Count; i++)
                {
                    //If friends
                    if (users_api[i].Friends.AreFriends(new List<long>() { Convert.ToInt64(user_id) })[0].FriendStatus == VkNet.Enums.FriendStatus.Friend)
                    {
                        try
                        {
                            user_albums.AddRange(users_api[i].Photo.GetAlbums(albumsGetParams).ToList());

                            if (user_albums.Count != 0)
                            {
                                for (int j = 0; j < user_albums.Count; j++)
                                {
                                    PhotoGetParams photoGetParams = new PhotoGetParams();
                                    photoGetParams.OwnerId = owner_id;
                                    photoGetParams.PhotoSizes = true;
                                    photoGetParams.AlbumId = PhotoAlbumType.Id((long)user_albums[j].Id);

                                    current_user_photos.AddRange(users_api[i].Photo.Get(photoGetParams).ToList());
                                }
                            }

                            current_user_photos.AddRange(users_api[i].Photo.Get(profilePhotosGetParams).ToList());
                            current_user_photos.AddRange(users_api[i].Photo.Get(wallPhotosGetParams).ToList());
                        }
                        catch (VkApiException ex)
                        {
                            applicationContract.OpenSpecialWindow(ex.Message);
                        }

                        break;
                    }
                }

                if (current_user_photos.Count == 0)
                {
                    for (int i = 0; i < users_api.Count; i++)
                    {
                        //If authorized user
                        if (users_api[i].UserId == Convert.ToInt64(user_id))
                        {
                            try
                            {
                                user_albums.AddRange(users_api[i].Photo.GetAlbums(albumsGetParams).ToList());

                                if (user_albums.Count != 0)
                                {
                                    for (int j = 0; j < user_albums.Count; j++)
                                    {
                                        PhotoGetParams photoGetParams = new PhotoGetParams();
                                        photoGetParams.OwnerId = owner_id;
                                        photoGetParams.PhotoSizes = true;
                                        photoGetParams.AlbumId = PhotoAlbumType.Id((long)user_albums[j].Id);

                                        current_user_photos.AddRange(users_api[i].Photo.Get(photoGetParams).ToList());
                                    }
                                }

                                current_user_photos.AddRange(users_api[i].Photo.Get(profilePhotosGetParams).ToList());
                                current_user_photos.AddRange(users_api[i].Photo.Get(wallPhotosGetParams).ToList());
                                current_user_photos.AddRange(users_api[i].Photo.Get(savedPhotosGetParams).ToList());
                            }
                            catch (VkApiException ex)
                            {
                                applicationContract.OpenSpecialWindow(ex.Message);
                            }

                            break;
                        }
                    }
                }

                current_photos_offset += photos_show_count;
            }
            else
            {
                current_photos_offset += photos_show_count;
            }

            if (current_user_photos.Count == 0)
            {
                applicationContract.ClearItemsFromPhotosList();
                applicationContract.SetPhotosListSatusData("0/0");
                return;
            }

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            int from_pos = (int)current_photos_offset - (int)photos_show_count;
            int to_pos = (int)current_photos_offset - 1;

            if (from_pos > current_user_photos.Count - 1) return;
            if (to_pos > current_user_photos.Count - 1) to_pos = current_user_photos.Count - 1;

            for (int i = from_pos; i <= to_pos; i++)
            {
                PhotosListItem photoItem = new PhotosListItem(current_user_photos[i].Sizes[current_user_photos[i].Sizes.Count - 1].Url);

                photosItems.Add(photoItem);
            }

            applicationContract.SetPhotosListSatusData(String.Format("{0}/{1}",to_pos+1,current_user_photos.Count));
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
