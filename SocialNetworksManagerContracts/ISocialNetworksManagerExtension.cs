using System;
using System.Collections.Generic;
using System.Windows;

using SocialNetworksManager.DataPresentation;

namespace SocialNetworksManager.Contracts
{
    public interface ISocialNetworksManagerExtension
    {
        String getExtensionName();
        String getSocialNetworkName();
        List<UserInfo> getAuthorizedUsers();

        void Authorization();
        void GetFriends();
        void GetPhotos();
        void GetGroups();
        void SendMessageToSelectedFriends();
    }
}