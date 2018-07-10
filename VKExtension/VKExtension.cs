using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using SocialNetworksManager.Contracts;

namespace VKExtension
{
    //Расширение для вконтакте
    [Export(typeof(ISocialNetworksManagerExtension))]
    public class VKExtension : ISocialNetworksManagerExtension
    {
        public string getExtensionName()
        {
            return "VK Extension";
        }

        public string getSocialNetworkName()
        {
            return "VK";
        }
    }
}
