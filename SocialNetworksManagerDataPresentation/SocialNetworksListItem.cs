using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SocialNetworksManager.DataPresentation
{
    public class SocialNetworksListItem : INotifyPropertyChanged
    {
        private String _name;
        private List<UserInfo> _authorized_users;

        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public List<UserInfo> AuthorizedUsers
        {
            get { return _authorized_users; }
            set
            {
                _authorized_users = value;
                OnPropertyChanged("AuthorizedUsers");
                OnPropertyChanged("AuthorizedUsersNames");
            }
        }

        public String AuthorizedUsersNames
        {
            get
            {
                StringBuilder names = new StringBuilder();

                for (int i = 0; i < _authorized_users.Count; i++)
                {
                    names.Append(_authorized_users[i].Name);
                    if (i != _authorized_users.Count - 1) names.Append(',');
                }

                return names.ToString();
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
