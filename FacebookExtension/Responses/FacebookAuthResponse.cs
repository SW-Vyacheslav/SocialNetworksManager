using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookExtension.Responses
{
    public class FacebookAuthResponse
    {
        public String AccessToken { get; set; }
        public String ExpiresIn { get; set; }
        public Boolean Error { get; set; }
    }
}
