using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKExtension.Responses
{
    public class VkAuthResponse
    {
        public String AccessToken { get; set; }
        public String UserId { get; set; }
        public String ExpiresIn { get; set; }
        public Boolean Error { get; set; }
    }
}
