using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworksManager.DataPresentation
{
    public class GroupsListItem
    {
        public String SocialNetworkName { get; set; }
        public UserInfo User { get; set; }
        public String GroupName { get; set; }
    }
}
