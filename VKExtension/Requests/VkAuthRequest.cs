using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKExtension.Enums;
using VKExtension.Responses;
using System.Windows.Controls;
using System.Reflection;

namespace VKExtension.Requests
{
    public class VkAuthRequest
    {
        public String              AppId        { get; set; }
        public VkAuthAppAccessRule Scope        { get; set; }
        public String              Version      { get; set; }
        public String              Revoke       { get; set; }

        public VkAuthRequest()
        {
            Revoke = "0";
        }
            
        public override string ToString()
        {
            return String.Format("https://oauth.vk.com/authorize?client_id={0}&redirect_uri=https://oauth.vk.com/blank.html&scope={1}&response_type=token&v={2}&revoke={3}", AppId, (int)Scope, Version, Revoke);
        }
    }
}
