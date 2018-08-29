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
        void ClearItemsFromPhotosList();
        void SetPhotosListSatusData(String data);

        List<FriendsListItem> GetFriendsListItems();
        String GetMessage();

        void AddSendMessageStatuses(List<SendMessageStatus> statuses);

        void OpenSpecialWindow(Uri auth_uri,Uri redirect_uri, Dictionary<String,String> parameters);
        void OpenSpecialWindow(UserControl userControl);
        void OpenSpecialWindow(String text);
        void CloseSpecialWindow();
    }
}
