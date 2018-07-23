using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKExtension
{
    public partial class VkAuthForm : Form
    {
        private Requests.VkAuthRequest request;
        private Responses.VkAuthResponse response;

        public VkAuthForm(Requests.VkAuthRequest request)
        {
            InitializeComponent();

            this.request = request;
            response = null;
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.Url.AbsoluteUri.Contains("https://oauth.vk.com/blank.html#"))
            {
                response = new Responses.VkAuthResponse();

                if (webBrowser.Url.AbsoluteUri.Contains("#access_token"))
                {
                    String data = webBrowser.Url.AbsoluteUri.Substring(webBrowser.Url.AbsoluteUri.IndexOf("access_token"));
                    String[] fields = data.Split('&');
                    response.AccessToken = fields[0].Split('=')[1];
                    response.ExpiresIn = fields[1].Split('=')[1];
                    response.UserId = fields[2].Split('=')[1];
                }
                else if (webBrowser.Url.AbsoluteUri.Contains("#error"))
                {
                    response.Error = true;
                }

                Close();
            }
        }

        public Responses.VkAuthResponse Authorize()
        {
            webBrowser.Navigate(request.ToString());
            ShowDialog();

            return response;
        }
    }
}
