using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using SocialNetworksManager.Contracts;

using SlackAPI;

using Helpers;

using Newtonsoft.Json;

namespace SlackExtension
{
    public class SlackHelper
    {
        private IApplicationContract applicationContract;
        private SlackGetTokenResponse getTokenResponse;

        public Boolean IsAuthorized
        {
            get
            {
                return getTokenResponse.Ok;
            }
        }

        public SlackHelper(IApplicationContract applicationContract)
        {
            this.applicationContract = applicationContract;
            getTokenResponse = null;
        }

        public void Authorize()
        {
            String state = Guid.NewGuid().ToString();
            SlackScope scopes = SlackScope.Identify | SlackScope.Post | SlackScope.Client | SlackScope.Read;
            
            Uri auth_uri = SlackClient.GetAuthorizeUri
            (
                Properties.AppSettings.Default.client_id,
                scopes,
                Properties.AppSettings.Default.redirect_uri,
                state
            );

            Dictionary<String, String> codeParams = new Dictionary<string, string>();
            codeParams["code"] = "";
            codeParams["state"] = "";
            codeParams["error"] = "";

            applicationContract.OpenSpecialWindow(auth_uri,new Uri(Properties.AppSettings.Default.redirect_uri),codeParams);

            if (codeParams["error"] != "") return;
            if (codeParams["state"] != state) return;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["client_id"] = Properties.AppSettings.Default.client_id;
            parameters["client_secret"] = Properties.AppSettings.Default.client_secret;
            parameters["redirect_uri"] = Properties.AppSettings.Default.redirect_uri;
            parameters["code"] = codeParams["code"];

            Uri token_get_uri = GetSlackApiUri("oauth.access", parameters);

            getTokenResponse = JsonConvert.DeserializeObject<SlackGetTokenResponse>(NetHelper.GetRequest(token_get_uri));
        }

        public List<User> GetUsers()
        {
            if (!getTokenResponse.Ok) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;

            Uri get_users_uri = GetSlackApiUri("users.list", parameters);

            UserListResponse userListResponse = JsonConvert.DeserializeObject<UserListResponse>(NetHelper.GetRequest(get_users_uri));

            return userListResponse.members.ToList();
        }

        public List<SlackFile> GetPhotos()
        {
            if (!getTokenResponse.Ok) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;
            parameters["types"] = "images";

            Uri get_files_uri = GetSlackApiUri("files.list", parameters);

            String data = NetHelper.GetRequest(get_files_uri);

            SlackFileListResponse fileListResponse = JsonConvert.DeserializeObject<SlackFileListResponse>(data);

            List<SlackFile> files = new List<SlackFile>();

            foreach (SlackFile file in fileListResponse.Files)
            {
                if (file.mimetype.Contains("image")) files.Add(file);
            }

            return files;
        }

        public static Uri GetSlackApiUri(String method, Dictionary<String,String> parameters)
        {
            StringBuilder uri = new StringBuilder();
            uri.AppendFormat("https://slack.com/api/{0}?",method);

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0) uri.Append('&');
                uri.AppendFormat("{0}={1}",parameters.ElementAt(i).Key,parameters.ElementAt(i).Value);
            }

            return new Uri(uri.ToString());
        }
    }
}
