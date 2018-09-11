using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Helpers;

using CefSharp.Wpf;
using CefSharp;

using MahApps.Metro.Controls;

namespace SocialNetworksManager
{
    public partial class SpecialWindow : MetroWindow
    {
        private ChromiumWebBrowser webBrowser;
        private Regex checkRegex;
        private Dictionary<String, String> parameters;

        private delegate void CloseWindowDelegate();

        public SpecialWindow(Uri uri, Uri redirect_uri, Dictionary<String, String> parameters)
        {
            InitializeComponent();

            checkRegex = new Regex("^" + redirect_uri.ToString());
            this.parameters = parameters;

            webBrowser = new ChromiumWebBrowser();
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            controlHolder.Children.Add(webBrowser);
            webBrowser.Address = uri.ToString();
        }

        private void WebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (checkRegex.Matches(e.Url).Count != 0)
            {
                Dictionary<String, String> uriParams = NetHelper.GetUriFields(new Uri(e.Url));

                if (uriParams.ContainsKey("error"))
                {
                    parameters["error"] = "true";
                    CloseWindow();
                    return;
                }

                for (int i = 0; i < parameters.Count; i++)
                {
                    for (int j = 0; j < uriParams.Count; j++)
                    {
                        if (parameters.ContainsKey(uriParams.ElementAt(j).Key))
                        {
                            parameters[uriParams.ElementAt(j).Key] = uriParams.ElementAt(j).Value;
                        }
                    }
                }

                CloseWindow();
            }
        }

        public SpecialWindow(UserControl userControl)
        {
            InitializeComponent();
            ShowCloseButton = false;
            ShowMinButton = false;
            ShowMaxRestoreButton = false;
            SizeToContent = SizeToContent.WidthAndHeight;

            controlHolder.Children.Add(userControl);
        }

        public SpecialWindow(String textBlockContent)
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;

            TextBlock label = new TextBlock();
            label.Text = textBlockContent;
            label.Margin = new Thickness(20);
            controlHolder.Children.Add(label);
        }

        public void CloseWindow()
        {
            Process.GetCurrentProcess().CloseMainWindow();
        }
    }
}