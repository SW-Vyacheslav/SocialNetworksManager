using System;
using System.Windows;

namespace SocialNetworksManager.Contracts
{
    public interface ISocialNetworksManagerExtension
    {
        String getExtensionName();
        String getSocialNetworkName();

        bool GetAuthStatus();

        void Authorization();
        void GetFriends();
        void GetPhotos();
        void SendMessage();
    }
}