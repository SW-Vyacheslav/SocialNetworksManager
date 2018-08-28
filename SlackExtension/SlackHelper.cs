using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SocialNetworksManager.Contracts;

using Helpers;

using Newtonsoft.Json;

namespace SlackExtension
{
    public class SlackHelper
    {
        private IApplicationContract applicationContract;
        private String _access_token;

        public Models.SlackUser User { get; set; }

        public Boolean IsAuthorized { get; set; }

        public SlackHelper(IApplicationContract applicationContract)
        {
            this.applicationContract = applicationContract;
            User = new Models.SlackUser();
        }

        public void Authorize()
        {
            String state = Guid.NewGuid().ToString();

            Dictionary<String, String> auth_parameters = new Dictionary<string, string>();
            auth_parameters["client_id"] = Properties.Resources.client_id;
            auth_parameters["scope"] = "files:read,chat:write:user,im:read,users:read";
            auth_parameters["redirect_uri"] = Properties.Resources.redirect_uri;
            auth_parameters["state"] = state;

            Uri auth_uri = GetAuthUri(auth_parameters);

            Dictionary<String, String> codeParams = new Dictionary<string, string>();
            codeParams["code"] = "";
            codeParams["state"] = "";
            codeParams["error"] = "";

            applicationContract.OpenSpecialWindow(auth_uri,new Uri(Properties.Resources.redirect_uri),codeParams);

            if (codeParams["error"] != "" || codeParams["code"] == "" || codeParams["state"] != state) return;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["client_id"] = Properties.Resources.client_id;
            parameters["client_secret"] = Properties.Resources.client_secret;
            parameters["redirect_uri"] = Properties.Resources.redirect_uri;
            parameters["code"] = codeParams["code"];

            Uri token_get_uri = GetApiUri("oauth.access", parameters);

            Responses.SlackGetTokenResponse getTokenResponse = JsonConvert.DeserializeObject<Responses.SlackGetTokenResponse>(NetHelper.GetRequest(token_get_uri));

            if (getTokenResponse.Ok)
            {
                IsAuthorized = true;
                _access_token = getTokenResponse.AccessToken;
                User.ID = getTokenResponse.UserID;
                User.RealName = GetUserInfo(getTokenResponse.UserID).RealName;
                User.TeamID = getTokenResponse.TeamID;
            }
        }

        public List<Models.SlackUser> GetUsers()
        {
            if (!IsAuthorized) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;

            Uri get_users_uri = GetApiUri("users.list", parameters);

            Responses.SlackUsersListResponse userListResponse = JsonConvert.DeserializeObject<Responses.SlackUsersListResponse>(NetHelper.GetRequest(get_users_uri));

            if (!userListResponse.Ok) return null;

            return userListResponse.Members.ToList();
        }

        public List<Models.SlackFile> GetPhotos(String user_id)
        {
            if (!IsAuthorized) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["types"] = "images";
            parameters["user"] = user_id;

            Uri get_files_uri = GetApiUri("files.list", parameters);

            Responses.SlackFilesListResponse fileListResponse = JsonConvert.DeserializeObject<Responses.SlackFilesListResponse>(NetHelper.GetRequest(get_files_uri));

            if (!fileListResponse.Ok) return null;

            return fileListResponse.Files.ToList();
        }

        public Boolean SendMessage(String channel_id,String message)
        {
            if (!IsAuthorized) return false;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["channel"] = channel_id;
            parameters["text"] = message;

            Uri send_message_uri = GetApiUri("chat.meMessage", parameters);

            Responses.SlackMeMessageResponse meMessageResponse = JsonConvert.DeserializeObject<Responses.SlackMeMessageResponse>(NetHelper.GetRequest(send_message_uri));

            if (!meMessageResponse.Ok) return false;
            else return true;
        }

        public List<Models.SlackIM> GetIms()
        {
            if (!IsAuthorized) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;

            Uri get_channels_uri = GetApiUri("im.list", parameters);

            Responses.SlackIMListResponse imlistResponse = JsonConvert.DeserializeObject<Responses.SlackIMListResponse>(NetHelper.GetRequest(get_channels_uri));
            
            if (!imlistResponse.Ok) return null;
            else return imlistResponse.IMs.ToList();
        }

        public Models.SlackUser GetUserInfo(String user_id)
        {
            if (!IsAuthorized) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["user"] = user_id;

            Uri users_info_url = GetApiUri("users.info",parameters);

            Responses.SlackUsersInfoResponse usersInfoResponse = JsonConvert.DeserializeObject<Responses.SlackUsersInfoResponse>(NetHelper.GetRequest(users_info_url));

            if (!usersInfoResponse.Ok) return null;
            else return usersInfoResponse.User;
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
