using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SocialNetworksManager.DataPresentation
{
    public class FriendsListItem : INotifyPropertyChanged
    {
        private Boolean _isChecked;

        public String SocialNetworkName { get; set; }
        public UserInfo Friend { get; set; }
        public UserInfo User { get; set; }

        public Boolean IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}