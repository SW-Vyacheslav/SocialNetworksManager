using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookExtension.Requests
{
    public class FacebookAuthRequest
    {
        public String AppId { get; set; }
        public String Version { get; set; }
        public Enums.FacebookAuthAppPermission Scope { get; set; }

        public override String ToString()
        {
            return String.Format("https://facebook.com/v{0}/dialog/oauth?response_type=token&display=page&client_id={1}&redirect_uri=https://www.facebook.com/connect/login_success.html&scope={2}",Version,AppId,Enums.FacebookAuthAppPermissionConverter.ConvertToString(Scope));
        }
    }
}
