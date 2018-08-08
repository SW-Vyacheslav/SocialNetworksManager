using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows;

using SocialNetworksManager.Contracts;
using SocialNetworksManager.DataPresentation;

using SlackAPI;
using System.Threading;

namespace SlackExtension
{
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class SlackExtension : ISocialNetworksManagerExtension
    {
        [Import(typeof(IApplicationContract),AllowRecomposition = true)]
        public IApplicationContract applicationContract;

        private SlackHelper slackHelper = null;

        public SlackExtension()
        {
            
        }

        public string getSocialNetworkName()
        {
            return "Slack";
        }

        public string getExtensionName()
        {
            return "Slack_Extension";
        }

        public bool GetAuthStatus()
        {
            if (slackHelper == null) return false;
            else return slackHelper.IsAuthorized;
        }

        public void Authorization()
        {
            if (GetAuthStatus()) return;

            slackHelper = new SlackHelper(applicationContract);
            slackHelper.Authorize();      
        }

        public void GetFriends()
        {
            if (!GetAuthStatus()) return;

            List<User> users = slackHelper.GetUsers();

            List<FriendsListItem> friendsItems = new List<FriendsListItem>();

            foreach (User user in users)
            {
                FriendsListItem friendItem = new FriendsListItem();
                friendItem.SocialNetworkName = getSocialNetworkName();
                friendItem.FriendName = user.profile.real_name;
                friendItem.Status = "Error";
                friendsItems.Add(friendItem);
            }

            applicationContract.AddItemsToFriendsList(friendsItems);
        }

        public void GetPhotos()
        {
            if (!GetAuthStatus()) return;

            List<SlackFile> photos = slackHelper.GetPhotos();

            List<PhotosListItem> photosItems = new List<PhotosListItem>();

            foreach (SlackFile photo in photos)
            {
                PhotosListItem photoItem = new PhotosListItem();
                photoItem.SocialNetworkName = getSocialNetworkName();
                photoItem.PhotoSource = photo.thumb_80;

                photosItems.Add(photoItem);
            }

            applicationContract.AddItemsToPhotosList(photosItems);
        }

        public void SendMessage()
        {
            
        }
    }
}
