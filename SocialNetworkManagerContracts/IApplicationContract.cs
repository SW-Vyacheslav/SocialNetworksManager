using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SocialNetworksManager.Contracts
{
    public interface IApplicationContract
    {
        void setInfoValue(String value);
        void setFriendsListItemsSource(IEnumerable<object> list);
        void setPhotosListItemsSource(IEnumerable<object> list);
        String getMessageText();
        object getSelectedItem();
    }
}
