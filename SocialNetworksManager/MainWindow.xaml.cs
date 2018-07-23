using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using SocialNetworksManager.Contracts;
using System.Text;
using System.Collections.Generic;

namespace SocialNetworksManager
{
    [Export(typeof(IApplicationContract))]
    public partial class MainWindow : Window, IApplicationContract
    {
        private DirectoryCatalog     directoryCatalog     = null;
        private CompositionContainer compositionContainer = null;
        private ImportManager        importManager        = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            RefreshExtensions();

            but_refreshExtensions.Click += But_refreshExtensions_Click;
            but_getvkFriends.Click      += But_getvkFriends_Click;
            but_getfbFriends.Click      += But_getfbFriends_Click;
        }

        #region UsualMethods
        private void InitializeContainer()
        {
            String dirPath = Environment.CurrentDirectory + "\\Extensions";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                MessageBox.Show("Extensions directory does not exists.\nExtensions did not loaded.");
                return;
            }
            if (Directory.GetFiles(dirPath).Length == 0)
            {
                MessageBox.Show("There are no extensions.");
                return;
            }

            directoryCatalog     =  new DirectoryCatalog(dirPath);
            compositionContainer =  new CompositionContainer(directoryCatalog);
            importManager        =  new ImportManager();

            compositionContainer.ComposeParts(this, importManager);
        }

        private void RefreshExtensions()
        {
            if (directoryCatalog == null) return;
            directoryCatalog.Refresh();
            socialNetworksHolder.Children.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                Button button   =  new Button();
                button.Content  =  extension.Value.getSocialNetworkName();
                button.Click   +=  Button_Click;

                socialNetworksHolder.Children.Add(button);
            }
        }

        private ISocialNetworksManagerExtension findSocialNetworkExtensionByName(String name)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                if (extension.Value.getSocialNetworkName().Equals(name))
                {
                    return extension.Value;
                }
            }

            return null;
        }
        #endregion

        #region ContractMethods
        public void setInfoValue(string value)
        {
            lbl_info.Content = value;
        }

        public void setFriendsListItemsSource(IEnumerable<object> list)
        {
            friendsList.ItemsSource = list;
        }
        #endregion

        #region EventMethods
        private void But_refreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private void But_getvkFriends_Click(object sender, RoutedEventArgs e)
        {
            findSocialNetworkExtensionByName("VK").GetFriends();
        }

        private void But_getfbFriends_Click(object sender, RoutedEventArgs e)
        {
            findSocialNetworkExtensionByName("Facebook").GetFriends();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            findSocialNetworkExtensionByName(((Button)sender).Content.ToString()).Authorization();
        }
        #endregion
    }
}