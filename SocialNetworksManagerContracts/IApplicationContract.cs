using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SocialNetworksManager.DataPresentation;

namespace SocialNetworksManager.Contracts
{

    public interface IApplicationContract
    {
        void AddItemsToFriendsList(List<FriendsListItem> items);
        void AddItemsToPhotosList(List<PhotosListItem> items);

        void OpenSpecialWindow(Uri auth_uri,Uri redirect_uri, Dictionary<String,String> parameters);
        void OpenSpecialWindow(UserControl userControl);
        void CloseSpecialWindow();
    }
}
