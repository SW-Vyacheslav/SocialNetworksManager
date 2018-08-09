using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using SocialNetworksManager.Contracts;

using Helpers;

using Newtonsoft.Json;

namespace SlackExtension
{
    public class SlackHelper
    {
        private IApplicationContract applicationContract;
        private Responses.SlackGetTokenResponse getTokenResponse;

        public Boolean IsAuthorized
        {
            get { return getTokenResponse.Ok; }
        }

        public SlackHelper(IApplicationContract applicationContract)
        {
            this.applicationContract = applicationContract;
            getTokenResponse = null;
        }

        public void Authorize()
        {
            String state = Guid.NewGuid().ToString();

            Dictionary<String, String> auth_parameters = new Dictionary<string, string>();
            auth_parameters["client_id"] = Properties.AppSettings.Default.client_id;
            auth_parameters["scope"] = "identify,post,client,read";
            auth_parameters["redirect_uri"] = Properties.AppSettings.Default.redirect_uri;
            auth_parameters["state"] = state;

            Uri auth_uri = GetAuthUri(auth_parameters);

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

            Uri token_get_uri = GetApiUri("oauth.access", parameters);

            getTokenResponse = JsonConvert.DeserializeObject<Responses.SlackGetTokenResponse>(NetHelper.GetRequest(token_get_uri));
        }

        public List<Models.SlackUser> GetUsers()
        {
            if (!getTokenResponse.Ok) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;

            Uri get_users_uri = GetApiUri("users.list", parameters);

            Responses.SlackUserListResponse userListResponse = JsonConvert.DeserializeObject<Responses.SlackUserListResponse>(NetHelper.GetRequest(get_users_uri));

            return userListResponse.Members.ToList();
        }

        public List<Models.SlackFile> GetPhotos()
        {
            if (!getTokenResponse.Ok) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;
            parameters["types"] = "images";

            Uri get_files_uri = GetApiUri("files.list", parameters);

            Responses.SlackFileListResponse fileListResponse = JsonConvert.DeserializeObject<Responses.SlackFileListResponse>(NetHelper.GetRequest(get_files_uri));

            List<Models.SlackFile> files = new List<Models.SlackFile>();

            foreach (Models.SlackFile file in fileListResponse.Files)
            {
                if (file.MimeType.Contains("image")) files.Add(file);
            }

            return files;
        }

        public Boolean SendMessage(String channel_id,String message)
        {
            if (!getTokenResponse.Ok) return false;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;
            parameters["channel"] = channel_id;
            parameters["text"] = message;

            Uri send_message_uri = GetApiUri("chat.meMessage", parameters);

            NetHelper.GetRequest(send_message_uri);

            return true;
        }

        public List<Models.SlackIM> GetIms()
        {
            if (!getTokenResponse.Ok) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = getTokenResponse.AccessToken;

            Uri get_channels_uri = GetApiUri("im.list", parameters);

            Responses.SlackIMListResponse listResponse = JsonConvert.DeserializeObject<Responses.SlackIMListResponse>(NetHelper.GetRequest(get_channels_uri));
            
            if (!listResponse.Ok) return null;
            else return listResponse.IMs.ToList();
        }

        public static Uri GetApiUri(String method, Dictionary<String,String> parameters)
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

        public static Uri GetAuthUri(Dictionary<String, String> parameters)
        {
            StringBuilder uri = new StringBuilder();
            uri.Append("https://slack.com/oauth/authorize?");

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0) uri.Append('&');
                uri.AppendFormat("{0}={1}", parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
            }

            return new Uri(uri.ToString());
        }
    }
}
