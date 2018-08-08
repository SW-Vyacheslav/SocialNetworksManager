using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Helpers;
using Microsoft.Win32;
using System.Diagnostics;


namespace SlackExtension
{
    public partial class AuthForm : Form
    {
        private Regex checkRegex;
        public SlackGetCodeResponse GetCodeResponse { get; set; }

        public AuthForm()
        {
            InitializeComponent();

            Boolean isEmulationModeSetted = CheckBrowserEmulationMode();

            if (!isEmulationModeSetted) SetBrowserEmulationMode();

            checkRegex = new Regex("^" + Properties.AppSettings.Default.redirect_uri);
            GetCodeResponse = null;

            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            webBrowser.ScriptErrorsSuppressed = true;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (checkRegex.Matches(e.Url.ToString()).Count != 0)
            {
                Dictionary<String, String> uriParams = NetHelper.GetUriFields(e.Url);

                GetCodeResponse = new SlackGetCodeResponse();

                if (uriParams.ContainsKey("error"))
                {
                    GetCodeResponse.Error = true;
                    Close();
                    return;
                }

                GetCodeResponse.Code = uriParams["code"];
                GetCodeResponse.State = uriParams["state"];
                Close();
            }
        }

        private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        public Boolean CheckBrowserEmulationMode()
        {
            RegistryKey key = Registry.CurrentUser;
            RegistryKey newKey = key.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Internet Explorer").OpenSubKey("Main").OpenSubKey("FeatureControl").OpenSubKey("FEATURE_BROWSER_EMULATION");
            string[] names = newKey.GetValueNames();

            if (names.Contains(System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName))) return true;
            else return false;
        }

        public void SetBrowserEmulationMode()
        {
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;
            UInt32 mode = 10000;
            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, mode);
        }

        public SlackGetCodeResponse StartAuthorization(Uri auth_uri)
        {
            webBrowser.Navigate(auth_uri);
            
            ShowDialog();

            return GetCodeResponse;
        }
    }
}
