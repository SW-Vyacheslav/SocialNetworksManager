using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworksManager.Contracts
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class AvailableInUIAttribute : Attribute
    {
        public String UIName = null;
    }

    //Контракт для взаимодействия с соц сетями 
    public interface ISocialNetworksManagerExtension
    {
        String getExtensionName();
        String getSocialNetworkName();

        void Authorization();

        void getFriends();
    }
}