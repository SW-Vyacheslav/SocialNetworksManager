using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

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
        private List<FriendsListItem>        friendsListItems        = new List<FriendsListItem>();
        private List<PhotosListItem>         photosListItems         = new List<PhotosListItem>();
        private List<SendMessageStatus>      messagesStatuses        = new List<SendMessageStatus>();

        private Thread checkConnectionThread;
        private SpecialWindow specialWindow;

        private delegate void SetNoConnectionPageVisibilityDelegate(Boolean isVisible);

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            RefreshExtensions();
            InitializeThreads();
        }

        #region UsualMethods
        private void SetNoConnectionPageVisibility(Boolean isVisible)
        {
            if (isVisible == true && noConnetionPage.Visibility != Visibility.Visible)
            {
                pages.Visibility = Visibility.Collapsed;
                noConnetionPage.Visibility = Visibility.Visible;
                button_refresh_extensions.IsEnabled = false;
            }
            else if (isVisible == false && noConnetionPage.Visibility != Visibility.Collapsed)
            {
                pages.Visibility = Visibility.Visible;
                noConnetionPage.Visibility = Visibility.Collapsed;
                button_refresh_extensions.IsEnabled = true;
            }
        }

        private void CheckConnectionThreadProc()
        {
            int description;
            SetNoConnectionPageVisibilityDelegate @delegate = new SetNoConnectionPageVisibilityDelegate(SetNoConnectionPageVisibility);

            while (true)
            {
                if (Helpers.NetHelper.InternetGetConnectedState(out description, 0)) Dispatcher.Invoke(@delegate, false);
                else Dispatcher.Invoke(@delegate, true);
            }
        }

        private void InitializeThreads()
        {
            checkConnectionThread = new Thread(CheckConnectionThreadProc);
            checkConnectionThread.IsBackground = true;

            checkConnectionThread.Start();
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

        private void UpdateFriends()
        {
            friendsList.ItemsSource = null;
            friendsListItems.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.GetFriends();
            }

            friendsList.ItemsSource = friendsListItems;
        }

        private void UpdatePhotos()
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

        public void AddSendMessageStatuses(List<SendMessageStatus> statuses)
        {
            messagesStatuses.AddRange(statuses);
        }
        #endregion

        #region EventMethods
        private void Button_RefreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            messagesStatuses.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.SendMessageToSelectedFriends();
            }

            StringBuilder messagesStatusesString = new StringBuilder();

            foreach (SendMessageStatus status in messagesStatuses)
            {
                messagesStatusesString.AppendFormat("{0}: Message to {1} {2}.\n",status.SocialNetworkName,status.UserName,status.IsMessageSended == true ? "Sended" : "Not Sended");
            }

            SpecialWindow specialWindow = new SpecialWindow(messagesStatusesString.ToString());
            specialWindow.ShowDialog();
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

        private void Button_SelectAllFriends_Click(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = true;
            }
        }

        private void Button_DeselectAllFriends_Click(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = false;
            }
        }

        private void Button_PhotoFullSize_Click(object sender, RoutedEventArgs e)
        {
            if (photosList.SelectedItems.Count == 0) return;
            bigPhoto.Visibility = Visibility.Visible;
            photosList.Visibility = Visibility.Collapsed;
            PhotosListItem list_item = photosList.SelectedItem as PhotosListItem;
            Uri source_uri = new Uri(list_item.PhotoSource);
            bigPhoto.Source = new BitmapImage(source_uri);
        }

        private void Button_PhotoFullSizeClose_Click(object sender, RoutedEventArgs e)
        {
            bigPhoto.Visibility = Visibility.Collapsed;
            photosList.Visibility = Visibility.Visible;
            bigPhoto.Source = null;
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is MetroAnimatedTabControl)
            {
                MetroAnimatedTabControl tabControl = sender as MetroAnimatedTabControl;
                MetroTabItem tabItem = tabControl.SelectedItem as MetroTabItem;

                switch (tabItem.Header)
                {
                    case "Friends":
                        UpdateFriends();
                        break;
                    case "Photos":
                        UpdatePhotos();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}