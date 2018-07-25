using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookExtension
{
    public partial class FacebookAuthForm : Form
    {
        private Requests.FacebookAuthRequest request;
        private Responses.FacebookAuthResponse response;

        public FacebookAuthForm(Requests.FacebookAuthRequest request)
        {
            InitializeComponent();

            this.request = request;
            response = null;
            webBrowser.Refresh(WebBrowserRefreshOption.Normal);
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(webBrowser.Url.AbsoluteUri.Contains("https://www.facebook.com/connect/login_success.html#"))
            {
                response = new Responses.FacebookAuthResponse();

                if (webBrowser.Url.AbsoluteUri.Contains("#access_token"))
                {
                    String data = webBrowser.Url.AbsoluteUri.Substring(webBrowser.Url.AbsoluteUri.IndexOf("access_token"));
                    String[] fields = data.Split('&');
                    response.AccessToken = fields[0].Split('=')[1];
                    response.ExpiresIn = fields[1].Split('=')[1];
                }
                else if (webBrowser.Url.AbsoluteUri.Contains("&error"))
                {
                    response.Error = true;
                }

                Close();
            }
        }

        public Responses.FacebookAuthResponse Authorize()
        {
            webBrowser.Navigate(request.ToString());
            ShowDialog();

            return response;
        }
    }
}
