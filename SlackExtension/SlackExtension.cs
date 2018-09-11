using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

        public SlackExtension()
        {
            users_helpers = new List<SlackHelper>();
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

                Responses.SlackUsersListResponse slackUsersListResponse = item.Users_List();

                if (!slackUsersListResponse.Ok) continue;

                List<Models.SlackUser> users = slackUsersListResponse.Members?.ToList();

                if (users == null) continue;

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

        public void GetPhotos()
        {
            SlackHelper slackHelper = null;

            for (int i = 0; i < users_helpers.Count; i++)
            {
                Responses.SlackUsersInfoResponse usersInfoResponse = users_helpers[i].Users_Info(applicationContract.GetPhotoUserID());

                if (usersInfoResponse.Ok)
                {
                    slackHelper = users_helpers[i];
                    break;
                }
            }

            if (!slackHelper.IsAuthorized) return;

            const UInt64 count = 20;
            UInt64 page = applicationContract.GetPhotosCount() / count + 1;

            Responses.SlackFilesListResponse slackFilesListResponse = slackHelper.Files_List(applicationContract.GetPhotoUserID(), Models.SlackFileTypes.Images, count, page);

            if (!slackFilesListResponse.Ok) return;

            List<Models.SlackFile> files = slackFilesListResponse.Files.ToList();

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            foreach (Models.SlackFile file in files)
            {
                if (!file.PublicURLShared) continue;

                String[] permalink_split = file.PermalinkPublic.Split('-');
                String photo_link = String.Format("{0}?pub_secret={1}", file.URLPrivate, permalink_split[permalink_split.Length - 1]);

                PhotosListItem photoItem = new PhotosListItem(new Uri(photo_link));

                photosItems.Add(photoItem);
            }

            applicationContract.AddItemsToPhotosList(photosItems);
            applicationContract.SetPhotosListSatusData(String.Format("{0}/{1}",applicationContract.GetPhotosCount(),slackFilesListResponse.Paging.Total));
            if (applicationContract.GetPhotosCount() == slackFilesListResponse.Paging.Total) applicationContract.DisableNextPhotosButton();
        }

        public void GetGroups()
        {
            foreach (SlackHelper user_helper in users_helpers)
            {
                if (!user_helper.IsAuthorized) continue;

                Responses.SlackChannelsListResponse slackChannelsListResponse = user_helper.Channels_List();

                if (!slackChannelsListResponse.Ok) continue;

                List<Models.SlackChannel> channels = slackChannelsListResponse.Channels?.ToList();

                if (channels == null) continue;

                List<GroupsListItem> groups_items = new List<GroupsListItem>();

                foreach (Models.SlackChannel channel in channels)
                {
                    if (!channel.IsMember) continue;

                    GroupsListItem group_item = new GroupsListItem();
                    group_item.GroupName = channel.Name;
                    group_item.SocialNetworkName = getSocialNetworkName();
                    group_item.User = new UserInfo()
                    {
                        SocialNetworkName = getSocialNetworkName(),
                        ID = user_helper.User.ID,
                        Name = user_helper.User.RealName
                    };

                    groups_items.Add(group_item);
                }

                applicationContract.AddItemsToGroupsList(groups_items);
            }
        }

        public void SendMessageToSelectedFriends()
        {
            foreach (SlackHelper user_helper in users_helpers)
            {
                if (!user_helper.IsAuthorized) continue;

                Responses.SlackIMListResponse slackIMListResponse = user_helper.Im_List();

                if (!slackIMListResponse.Ok) continue;

                List<Models.SlackIM> ims = slackIMListResponse.IMs?.ToList();

                if (ims == null) continue;

                List<FriendsListItem> friends_items = applicationContract.GetFriendsListItems();
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
                            break;
                        }
                    }

                    if (channel_id == null) continue;

                    SendMessageStatus status = new SendMessageStatus();
                    status.SocialNetworkName = getSocialNetworkName();
                    status.UserNameTo = friend_item.Friend.Name;
                    status.UserNameFrom = friend_item.User.Name;
                    status.IsMessageSended = user_helper.Chat_MeMessage(channel_id, applicationContract.GetMessage()).Ok;
                    statuses.Add(status);
                }

                applicationContract.AddSendMessageStatuses(statuses);
            }
        }
    }
}
