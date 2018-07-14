using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworksManager.Contracts
{
    //Контракт для взаимодействия с соц сетями 
    public interface ISocialNetworksManagerExtension
    {
        String getExtensionName();
        String getSocialNetworkName();

        void Authorization();
    }
}