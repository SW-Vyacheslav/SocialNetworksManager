using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

using SocialNetworksManager.Contracts;

using Helpers;

using CefSharp;
using CefSharp.Wpf;

namespace SocialNetworksManager
{
    /// <summary>
    /// Interaction logic for SpecialWindow.xaml
    /// </summary>
    public partial class SpecialWindow : Window
    {
        private ChromiumWebBrowser webBrowser;
        private Regex checkRegex;
        private Dictionary<String, String> parameters;
        private Boolean isCloseWindow = false;

        private delegate void CloseWindowDelegate();

        public SpecialWindow(Uri uri, Uri redirect_uri, Dictionary<String, String> parameters)
        {
            InitializeComponent();

            Closing += SpecialWindow_Closing;
            checkRegex = new Regex("^" + redirect_uri.ToString());
            this.parameters = parameters;

            InitBrowser();
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            webBrowser.Address = uri.ToString();
            Thread checkThread = new Thread(checkThreadProc);
            checkThread.IsBackground = true;
            checkThread.Start();
        }

        public SpecialWindow(UserControl userControl)
        {
            InitializeComponent();
            Height = userControl.Height*2;
            controlHolder.Children.Add(userControl);
        }

        private void checkThreadProc()
        {
            while(true)
            {
                if(isCloseWindow)
                {
                    InvokeCloseWindow();
                    break;
                }
                Thread.Sleep(50);
            }
        }

        private void InitBrowser()
        {
            CefSettings settings = new CefSettings();

            Cef.Initialize(settings);

            webBrowser = new ChromiumWebBrowser();
            
            controlHolder.Children.Add(webBrowser);
        }

        private void SpecialWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cef.Shutdown();
        }

        private void InvokeCloseWindow()
        {
            CloseWindowDelegate closeWindowDelegate = new CloseWindowDelegate(Close);
            Dispatcher.Invoke(closeWindowDelegate);
        }

        private void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (checkRegex.Matches(e.Url).Count != 0)
            {
                Dictionary<String, String> uriParams = NetHelper.GetUriFields(new Uri(e.Url));

                if (uriParams.ContainsKey("error"))
                {
                    parameters["error"] = "true";
                    isCloseWindow = true;
                    return;
                }

                for (int i = 0; i < parameters.Count; i++)
                {
                    for (int j = 0; j < uriParams.Count; j++)
                    {
                        if(parameters.ContainsKey(uriParams.ElementAt(j).Key))
                        {
                            parameters[uriParams.ElementAt(j).Key] = uriParams.ElementAt(j).Value;
                        }
                    }
                }

                isCloseWindow = true;
            }
        }
    }
}
