using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using SocialNetworksManager.Contracts;
using SocialNetworksManager.DataPresentation;

namespace SlackExtension
{
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class SlackExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract),AllowRecomposition = true)]
        public IApplicationContract applicationContract;

        //For many users
        private List<SlackHelper> users_helpers = null;

        //Fro photos
        private String current_photos_user_id = "";
        private Int64 photos_show_count = 20;
        private Int64 current_photos_offset = 0;
        private List<Models.SlackFile> current_user_photos = null;

        public SlackExtension()
        {
            users_helpers = new List<SlackHelper>();
            current_user_photos = new List<Models.SlackFile>();
        }

        public string getSocialNetworkName()
        {
            return "Slack";
        }

        public string getExtensionName()
        {
            return "Slack_Extension";
        }

        public List<UserInfo> getAuthorizedUsers()
        {
            List<UserInfo> users = new List<UserInfo>();

            for (int i = 0; i < users_helpers.Count; i++)
            {
                if (!users_helpers[i].IsAuthorized) continue;

                UserInfo userInfo = new UserInfo();
                userInfo.Name = users_helpers[i].User.RealName;
                userInfo.ID = users_helpers[i].User.ID;
                userInfo.SocialNetworkName = getSocialNetworkName();
                users.Add(userInfo);
            }

            return users;
        }

        public void Authorization()
        {
            SlackHelper slackHelper = new SlackHelper(applicationContract);
            slackHelper.Authorize();

            if (!slackHelper.IsAuthorized)
            {
                applicationContract.OpenSpecialWindow("Slack Auth Error.");
                return;
            }

            foreach (SlackHelper item in users_helpers)
            {
                if (item.User.ID == slackHelper.User.ID)
                {
                    applicationContract.OpenSpecialWindow("You already authorized");
                    return;
                }
            }

            users_helpers.Add(slackHelper);
        }

        public void GetFriends()
        {
            foreach (SlackHelper item in users_helpers)
            {
                if (!item.IsAuthorized) continue;

                List<Models.SlackUser> users = item.GetUsers();

                List<FriendsListItem> friendsItems = new List<FriendsListItem>();

                foreach (Models.SlackUser user in users)
                {
                    FriendsListItem friendItem = new FriendsListItem();
                    friendItem.User = new UserInfo();
                    friendItem.Friend = new UserInfo();
                    friendItem.SocialNetworkName = getSocialNetworkName();
                    friendItem.Friend.Name = user.Profile.RealName;
                    friendItem.Friend.ID = user.ID;
                    friendItem.User.ID = item.User.ID;
                    friendItem.User.Name = item.User.RealName;
                    friendsItems.Add(friendItem);
                }

                applicationContract.AddItemsToFriendsList(friendsItems);
            }
        }

        public void RefreshPhotos(string user_id)
        {
            applicationContract.ClearItemsFromPhotosList();
            current_photos_user_id = "";
            current_photos_offset = 0;
            GetPhotos(user_id);
        }

        public void GetPhotos(string user_id)
        {
            if(current_photos_user_id != user_id)
            {
                SlackHelper slackHelper = null;

                current_photos_user_id = user_id;
                current_photos_offset = 0;

                current_user_photos.Clear();
                applicationContract.ClearItemsFromPhotosList();

                for (int i = 0; i < users_helpers.Count; i++)
                {
                    if (users_helpers[i].GetUserInfo(user_id) != null)
                    {
                        slackHelper = users_helpers[i];
                        break;
                    }
                }

                current_user_photos = slackHelper.GetPhotos(user_id);

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
                String[] permalink_split = current_user_photos[i].PermalinkPublic.Split('-');
                String photo_link = String.Format("{0}?pub_secret={1}", current_user_photos[i].URLPrivate, permalink_split[permalink_split.Length - 1]);

                PhotosListItem photoItem = new PhotosListItem(new Uri(photo_link));

                photosItems.Add(photoItem);
            }

            applicationContract.SetPhotosListSatusData(String.Format("{0}/{1}", to_pos + 1, current_user_photos.Count));
            applicationContract.AddItemsToPhotosList(photosItems);
        }

        public void SendMessageToSelectedFriends()
        {
            foreach (SlackHelper user_helper in users_helpers)
            {
                if (!user_helper.IsAuthorized) continue;

                List<FriendsListItem> friends_items = applicationContract.GetFriendsListItems();
                List<Models.SlackIM> ims = user_helper.GetIms();
                List<SendMessageStatus> statuses = new List<SendMessageStatus>();

                foreach (FriendsListItem friend_item in friends_items)
                {
                    if (!friend_item.SocialNetworkName.Equals(getSocialNetworkName())) continue;
                    if (!friend_item.IsChecked) continue;
                    if (friend_item.User.ID != user_helper.User.ID) continue;

                    String channel_id = null;

                    foreach (Models.SlackIM im in ims)
                    {
                        if (friend_item.Friend.ID == im.User)
                        {
                            channel_id = im.ID;
                        }
                    }

                    SendMessageStatus status = new SendMessageStatus();
                    status.SocialNetworkName = getSocialNetworkName();
                    status.UserNameTo = friend_item.Friend.Name;
                    status.UserNameFrom = friend_item.User.Name;
                    status.IsMessageSended = user_helper.SendMessage(channel_id, applicationContract.GetMessage());
                    statuses.Add(status);
                }

                applicationContract.AddSendMessageStatuses(statuses);
            }
        }
    }
}
