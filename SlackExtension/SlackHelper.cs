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
            auth_parameters["scope"] = "files:read,chat:write:user,im:read,users:read,channels:read";
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
                User = Users_Info(getTokenResponse.UserID).User;
            }
        }

        public Responses.SlackUsersListResponse Users_List()
        {
            if (!IsAuthorized) throw new Exceptions.SlackAuthException("Not Authorized."); ;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;

            Uri users_list_uri = GetApiUri("users.list", parameters);

            Responses.SlackUsersListResponse userListResponse = JsonConvert.DeserializeObject<Responses.SlackUsersListResponse>(NetHelper.GetRequest(users_list_uri));

            return userListResponse;
        }

        public Responses.SlackFilesListResponse Files_List
        (
            String user_id = null,
            Models.SlackFileType file_types = null,
            UInt64 count = 100,
            UInt64 page = 1,
            String channel_id = null,
            String ts_from = null,
            String ts_to = null
        )
        {
            if (!IsAuthorized) return null;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["types"] = file_types.ToString();
            parameters["user"] = user_id;
            parameters["channel"] = channel_id;
            parameters["count"] = Convert.ToString(count);
            parameters["page"] = Convert.ToString(page);
            parameters["ts_from"] = ts_from;
            parameters["ts_to"] = ts_to;

            Uri files_list_uri = GetApiUri("files.list", parameters);

            Responses.SlackFilesListResponse fileListResponse = JsonConvert.DeserializeObject<Responses.SlackFilesListResponse>(NetHelper.GetRequest(files_list_uri));

            return fileListResponse;
        }

        public Responses.SlackChatMeMessageResponse Chat_MeMessage(String channel_id,String message)
        {
            if (!IsAuthorized) throw new Exceptions.SlackAuthException("Not Authorized.");

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["channel"] = channel_id;
            parameters["text"] = message;

            Uri chat_memessage_uri = GetApiUri("chat.meMessage", parameters);

            Responses.SlackChatMeMessageResponse meMessageResponse = JsonConvert.DeserializeObject<Responses.SlackChatMeMessageResponse>(NetHelper.GetRequest(chat_memessage_uri));

            return meMessageResponse;
        }

        public Responses.SlackIMListResponse Im_List()
        {
            if (!IsAuthorized) throw new Exceptions.SlackAuthException("Not Authorized."); 

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;

            Uri im_list_uri = GetApiUri("im.list", parameters);

            Responses.SlackIMListResponse imlistResponse = JsonConvert.DeserializeObject<Responses.SlackIMListResponse>(NetHelper.GetRequest(im_list_uri));

            return imlistResponse;
        }

        public Responses.SlackUsersInfoResponse Users_Info(String user_id, Boolean include_locale = false)
        {
            if (!IsAuthorized) throw new Exceptions.SlackAuthException("Not Authorized."); ;

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;
            parameters["user"] = user_id;
            parameters["include_locale"] = include_locale == false ? "false" : "true";

            Uri users_info_url = GetApiUri("users.info",parameters);

            Responses.SlackUsersInfoResponse usersInfoResponse = JsonConvert.DeserializeObject<Responses.SlackUsersInfoResponse>(NetHelper.GetRequest(users_info_url));

            return usersInfoResponse;
        }

        public Responses.SlackChannelsListResponse Channels_List()
        {
            if (!IsAuthorized) throw new Exceptions.SlackAuthException("Not Authorized.");

            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters["token"] = _access_token;

            Uri channels_list_uri = GetApiUri("channels.list",parameters);

            Responses.SlackChannelsListResponse slackChannelsListResponse = JsonConvert.DeserializeObject<Responses.SlackChannelsListResponse>(NetHelper.GetRequest(channels_list_uri));

            return slackChannelsListResponse;
        }

        public static Uri GetApiUri(String method, Dictionary<String,String> parameters)
        {
            StringBuilder uri = new StringBuilder();
            uri.AppendFormat("https://slack.com/api/{0}?",method);

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters.ElementAt(i).Value != null)
                {
                    if (i != 0) uri.Append('&');
                    uri.AppendFormat("{0}={1}", parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
                }  
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
