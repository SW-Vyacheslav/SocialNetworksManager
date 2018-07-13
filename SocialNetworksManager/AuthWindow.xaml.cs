using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SocialNetworksManager
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public String access_token { get; private set; }

        public AuthWindow(String auth_link)
        {
            InitializeComponent();

            browser.LoadCompleted += Browser_LoadCompleted;

            browser.Navigate(auth_link);
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            mshtml.HTMLDocument document = (mshtml.HTMLDocument)browser.Document;
            
            if(document.title.Equals("OAuth Blank"))
            {
                access_token = document.url.Substring(document.url.IndexOf('=')+1, document.url.IndexOf('&') - (document.url.IndexOf('=') + 1));
                Close();
            }
        }
    }
}
