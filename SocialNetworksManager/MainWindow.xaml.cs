using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SocialNetworksManager.Contracts;
using SocialNetworksManager.DataPresentation;

using MahApps.Metro.Controls;

namespace SocialNetworksManager
{
    [Export(typeof(IApplicationContract))]
    public partial class MainWindow : MetroWindow, IApplicationContract
    {
        private DirectoryCatalog     directoryCatalog     = null;
        private CompositionContainer compositionContainer = null;
        private ImportManager        importManager        = null;

        private List<SocialNetworksListItem> socialNetworksListItems = new List<SocialNetworksListItem>();
        private List<FriendsListItem> friendsListItems = new List<FriendsListItem>();
        private List<PhotosListItem> photosListItems = new List<PhotosListItem>();

        private SpecialWindow specialWindow;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            RefreshExtensions();
        }

        #region UsualMethods
        private void SetDllsDirectory(String path)
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + path);
        }

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

            try
            {
                compositionContainer.ComposeParts(this, importManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
                Environment.Exit(1);
            }
        }

        private void RefreshExtensions()
        {
            if (directoryCatalog == null) return;
            directoryCatalog.Refresh();
            socialNetworksHolder.ItemsSource = null;

            List<SocialNetworksListItem> oldList = socialNetworksListItems;
            socialNetworksListItems = new List<SocialNetworksListItem>();
            
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                SocialNetworksListItem methodsItem = new SocialNetworksListItem();
                methodsItem.Name = extension.Value.getSocialNetworkName();
                methodsItem.Status = extension.Value.GetAuthStatus() == true ? "Authorized" : "Not Authorized";

                foreach (SocialNetworksListItem item in oldList)
                {
                    if (methodsItem.Name == item.Name) methodsItem.IsButtonEnabled = item.IsButtonEnabled;
                }
                
                socialNetworksListItems.Add(methodsItem);
            }

            socialNetworksHolder.ItemsSource = socialNetworksListItems;
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
        public void AddItemsToFriendsList(List<FriendsListItem> items)
        {
            friendsListItems.AddRange(items);
        }

        public void AddItemsToPhotosList(List<PhotosListItem> items)
        {
            photosListItems.AddRange(items);
        }

        public void OpenSpecialWindow(Uri uri, Uri redirect_uri, Dictionary<String, String> parameters)
        {
            specialWindow = new SpecialWindow(uri,redirect_uri,parameters);
            specialWindow.ShowDialog();
        }

        public void OpenSpecialWindow(UserControl userControl)
        {
            specialWindow = new SpecialWindow(userControl);
            specialWindow.ShowDialog();
        }

        public void CloseSpecialWindow()
        {
            specialWindow.Close();
        }

        public List<FriendsListItem> GetFriendsListItems()
        {
            return friendsListItems;
        }

        public string GetMessage()
        {
            return message_text_box.Text;
        }
        #endregion

        #region EventMethods
        private void Button_RefreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.SendMessageToSelectedFriends();
            }
        }

        private void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SocialNetworksListItem item = (button).DataContext as SocialNetworksListItem;
            ISocialNetworksManagerExtension extension = findSocialNetworkExtensionByName(item.Name);
            extension.Authorization();
            if (extension.GetAuthStatus())
            {
                item.Status = "Authorized";
                item.IsButtonEnabled = false;
                button.IsEnabled = false;
            }
        }

        private void Button_Select_All_Friends(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = true;
            }
        }

        private void Button_Deselect_All_Friends(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = false;
            }
        }

        private void Button_Update_Friends(object sender, RoutedEventArgs e)
        {
            friendsList.ItemsSource = null;
            friendsListItems.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.GetFriends();
            }

            friendsList.ItemsSource = friendsListItems;
        }

        private void Button_Update_Photos(object sender, RoutedEventArgs e)
        {
            photosList.ItemsSource = null;
            photosListItems.Clear();
            
            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.GetPhotos();
            }

            photosList.ItemsSource = photosListItems;
        }
        #endregion
    }
}